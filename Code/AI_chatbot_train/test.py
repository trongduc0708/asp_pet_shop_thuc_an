import torch
import torch.nn as nn
from torch.utils.data import Dataset, DataLoader
import pandas as pd
import numpy as np
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.preprocessing import OneHotEncoder
from sklearn.metrics.pairwise import cosine_similarity

# CVAE-CF (Collaborative Filtering)

class UserItemDataset(Dataset):
    def __init__(self, csv_path):
        df = pd.read_csv(csv_path)
        self.user2idx = {u: i for i, u in enumerate(sorted(df["user_id"].unique()))}
        self.item2idx = {it: i for i, it in enumerate(sorted(df["item_id"].unique()))}
        n_users, n_items = len(self.user2idx), len(self.item2idx)

        df["weight"] = df["event_type"].map({"view": 1.0, "add_to_cart": 2.0, "purchase": 3.0}).fillna(1.0)
        self.user_vectors = torch.zeros((n_users, n_items), dtype=torch.float32)
        for _, row in df.iterrows():
            u = self.user2idx[row["user_id"]]
            i = self.item2idx[row["item_id"]]
            w = row["weight"]
            self.user_vectors[u, i] = max(self.user_vectors[u, i].item(), w)

    def __len__(self):
        return self.user_vectors.size(0)

    def __getitem__(self, idx):
        return self.user_vectors[idx]


class CVAECF(nn.Module):
    def __init__(self, input_dim, latent_dim=32):
        super().__init__()
        self.enc_fc1 = nn.Linear(input_dim, 256)
        self.enc_mu = nn.Linear(256, latent_dim)
        self.enc_logvar = nn.Linear(256, latent_dim)
        self.dec_fc1 = nn.Linear(latent_dim, 256)
        self.dec_out = nn.Linear(256, input_dim)

    def encode(self, x):
        h = torch.relu(self.enc_fc1(x))
        return self.enc_mu(h), self.enc_logvar(h)

    def reparameterize(self, mu, logvar):
        std = torch.exp(0.5 * logvar)
        eps = torch.randn_like(std)
        return mu + eps * std

    def decode(self, z):
        h = torch.relu(self.dec_fc1(z))
        return torch.sigmoid(self.dec_out(h))

    def forward(self, x):
        mu, logvar = self.encode(x)
        z = self.reparameterize(mu, logvar)
        recon = self.decode(z)
        return recon, mu, logvar


def cf_loss(recon_x, x, mu, logvar):
    bce = nn.functional.binary_cross_entropy(recon_x, (x > 0).float(), reduction="sum")
    kld = -0.5 * torch.sum(1 + logvar - mu.pow(2) - logvar.exp())
    return bce + kld


def train_cvae_cf(csv_path="interactions_cvae_cf_large.csv", model_path="cvae_cf_model.pt",
                  latent_dim=32, epochs=10, lr=1e-3, batch_size=64):
    dataset = UserItemDataset(csv_path)
    loader = DataLoader(dataset, batch_size=batch_size, shuffle=True)
    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    model = CVAECF(input_dim=dataset.user_vectors.size(1), latent_dim=latent_dim).to(device)
    opt = torch.optim.Adam(model.parameters(), lr=lr)

    model.train()
    for epoch in range(epochs):
        total = 0
        for batch in loader:
            batch = batch.to(device)
            recon, mu, logvar = model(batch)
            loss = cf_loss(recon, batch, mu, logvar)
            opt.zero_grad(); loss.backward(); opt.step()
            total += loss.item()
        print(f"[CF] Epoch {epoch+1}/{epochs} Loss: {total/len(dataset):.4f}")

    torch.save({
        "model_state": model.state_dict(),
        "user2idx": dataset.user2idx,
        "item2idx": dataset.item2idx,
    }, model_path)
    print("✅ Saved CF model to", model_path)


# CVAE-CBF (Content-Based)

class ItemDataset(Dataset):
    def __init__(self, csv_path):
        df = pd.read_csv(csv_path)
        self.df = df
        tfidf = TfidfVectorizer(max_features=200)
        desc_vec = tfidf.fit_transform(df["description"].fillna("")).toarray()

        cat_data = df[["category", "brand", "pet_type"]].fillna("unk")
        ohe = OneHotEncoder(sparse_output=False)
        cat_vec = ohe.fit_transform(cat_data)

        price = df["price"].fillna(0).to_numpy(dtype=np.float32).reshape(-1, 1)
        price = (price - price.mean()) / (price.std() + 1e-6)
        feats = np.concatenate([desc_vec, cat_vec, price], axis=1).astype(np.float32)
        self.features = torch.from_numpy(feats)

    def __len__(self):
        return self.features.size(0)

    def __getitem__(self, idx):
        return self.features[idx]


class CVAECBF(nn.Module):
    def __init__(self, input_dim, latent_dim=32):
        super().__init__()
        self.enc1 = nn.Linear(input_dim, 256)
        self.enc_mu = nn.Linear(256, latent_dim)
        self.enc_logvar = nn.Linear(256, latent_dim)
        self.dec1 = nn.Linear(latent_dim, 256)
        self.dec_out = nn.Linear(256, input_dim)

    def encode(self, x):
        h = torch.relu(self.enc1(x))
        return self.enc_mu(h), self.enc_logvar(h)

    def reparameterize(self, mu, logvar):
        std = torch.exp(0.5 * logvar)
        eps = torch.randn_like(std)
        return mu + eps * std

    def decode(self, z):
        h = torch.relu(self.dec1(z))
        return self.dec_out(h)

    def forward(self, x):
        mu, logvar = self.encode(x)
        z = self.reparameterize(mu, logvar)
        recon = self.decode(z)
        return recon, mu, logvar


def cbf_loss(recon_x, x, mu, logvar):
    mse = nn.functional.mse_loss(recon_x, x, reduction="sum")
    kld = -0.5 * torch.sum(1 + logvar - mu.pow(2) - logvar.exp())
    return mse + kld


def train_cvae_cbf(csv_path="items_cvae_cbf.csv", model_path="cvae_cbf_model.pt",
                   latent_dim=32, epochs=10, lr=1e-3, batch_size=32):
    dataset = ItemDataset(csv_path)
    loader = DataLoader(dataset, batch_size=batch_size, shuffle=True)
    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    model = CVAECBF(input_dim=dataset.features.size(1), latent_dim=latent_dim).to(device)
    opt = torch.optim.Adam(model.parameters(), lr=lr)

    model.train()
    for epoch in range(epochs):
        total = 0
        for batch in loader:
            batch = batch.to(device)
            recon, mu, logvar = model(batch)
            loss = cbf_loss(recon, batch, mu, logvar)
            opt.zero_grad(); loss.backward(); opt.step()
            total += loss.item()
        print(f"[CBF] Epoch {epoch+1}/{epochs} Loss: {total/len(dataset):.4f}")

    torch.save({
        "model_state": model.state_dict(),
    }, model_path)
    print("✅ Saved CBF model to", model_path)


# HYBRID (CF + CBF + CVE)

class HybridRecommender:
    def __init__(self, cf_path, cbf_path, cf_data, cbf_data, alpha=0.6):
        cf_ds = UserItemDataset(cf_data)
        self.cf = CVAECF(input_dim=cf_ds.user_vectors.size(1))
        cf_ckpt = torch.load(cf_path, map_location="cpu", weights_only=False)
        self.cf.load_state_dict(cf_ckpt["model_state"])
        self.cf.eval()
        self.user_vectors = cf_ds.user_vectors
        self.user2idx = cf_ckpt["user2idx"]
        self.item2idx = cf_ckpt["item2idx"]
        self.idx2item = {v: k for k, v in self.item2idx.items()}

        cbf_ds = ItemDataset(cbf_data)
        self.cbf = CVAECBF(input_dim=cbf_ds.features.size(1))
        cbf_ckpt = torch.load(cbf_path, map_location="cpu", weights_only=False)
        self.cbf.load_state_dict(cbf_ckpt["model_state"])
        self.cbf.eval()
        self.item_features = cbf_ds.features
        self.items_df = cbf_ds.df
        self.alpha = alpha

    def _cf_score(self, user_id):
        if user_id not in self.user2idx:
            return np.zeros(len(self.item2idx))
        u = self.user_vectors[self.user2idx[user_id]].unsqueeze(0)
        with torch.no_grad():
            mu, _ = self.cf.encode(u)
            recon = self.cf.decode(mu).numpy().flatten()
        return recon

    def _cbf_score(self, item_id):
        idx = np.where(self.items_df["item_id"] == item_id)[0]
        if len(idx) == 0:
            return np.zeros(self.item_features.shape[0])
        i = idx[0]
        with torch.no_grad():
            mu, _ = self.cbf.encode(self.item_features)
        sims = cosine_similarity(mu[i:i+1], mu).flatten()
        return sims

    def hybrid_recommend(self, user_id, current_item=None, k=5):
        cf_score = self._cf_score(user_id)
        cbf_score = self._cbf_score(current_item) if current_item else np.zeros_like(cf_score)
        hybrid = self.alpha * cf_score + (1 - self.alpha) * cbf_score
        top = np.argsort(hybrid)[::-1][:k]
        return [(self.idx2item[i], float(hybrid[i])) for i in top]




if __name__ == "__main__":
    # test hybrid recommender
    hybrid = HybridRecommender(
        cf_path="cvae_cf_model.pt",
        cbf_path="cvae_cbf_model.pt",
        cf_data="interactions_cvae_cf_large.csv",
        cbf_data="items_cvae_cbf.csv",
        alpha=0.7
    )

    print("Gợi ý kết hợp cho user 5 (đang xem Whiskas):")
    print(hybrid.hybrid_recommend(user_id=5, current_item="WHISKAS-ADULT-001", k=5))
