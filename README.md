# Sumask

Prototipe game kartu turn-based yang dibuat menggunakan Unity. Proyek ini berisi sistem kartu, musuh, hero, damage, shield, mana, dan giliran bertarung.

## Persyaratan

- Unity **6000.0.59f2** atau versi editor yang kompatibel
- Unity Hub

## Cara Membuka Proyek

1. Buka Unity Hub.
2. Pilih **Add** atau **Add project from disk**.
3. Pilih folder `slay the spire`.
4. Buka proyek dengan Unity **6000.0.59f2**.
5. Buka `Assets/Scenes/SampleScene.unity`.
6. Tekan **Play** di Unity Editor.

## Fitur Utama

- Sistem kartu dan kartu di tangan
- Sistem giliran dan aksi game
- Mana untuk memainkan kartu
- Serangan, damage, dan shield
- Musuh dengan pola giliran
- Target musuh acak atau semua musuh
- Tampilan kartu saat hover
- Scene akhir permainan

## Struktur Proyek

```text
Assets/
  Scenes/          Scene permainan
  Script/
    Creators/      Pembuatan tampilan kartu dan musuh
    Data/          Data kartu, hero, dan musuh
    Effects/       Efek kartu
    GameAction/    Aksi yang dijalankan dalam sistem game
    General/       Sistem aksi dan utilitas umum
    Models/        Model kartu, efek, dan target
    Systems/       Sistem inti permainan
    TargetModes/   Cara pemilihan target
    Ui/            Komponen antarmuka
    Views/         Tampilan kartu dan karakter
  Settings/        Pengaturan rendering dan proyek
Packages/          Dependensi Unity
ProjectSettings/   Konfigurasi proyek Unity
```

## Menjalankan dari Source

Klon repositori, lalu buka folder proyek melalui Unity Hub:

```bash
git clone https://github.com/USERNAME/NAMA-REPO.git
```

Ganti `USERNAME` dan `NAMA-REPO` dengan username dan nama repositori GitHub Anda.
