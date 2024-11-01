# WebScreen


## Apa ini?
Sebuah aplikasi untuk mencerminkan layar Windows ke perangkat lain melalui internet lokal dan juga untuk mensimulasikan touch input dari perangkat client ke Windows.

Dapat digunakan untuk membantu developer menguji aplikasi yang dibuat untuk dijalankan pada perangkat Windows layar sentuh menggunakan perangkat lain yang memiliki layar sentuh (seperti smartphone). Cocok apabila layar perangkat Windows Anda tidak memiliki sensor layar sentuh.

Pertama kali dibuat awalnya karena penasaran saja bagaimana Windows bekerja apabila menggunakan layar sentuh.

Dibuat pakai WPF. Untuk sisi client dibuat menggunakan native HTML.

## Bagaimana cara kerjanya?
Tidak terlalu kompleks,

Saat aplikasi dijalankan, WebScreen akan menjalankan server HTTP lokal untuk mengirimkan tampilan web bagi perangkat Client dan juga server Websocket untuk transmisi gambar layar dari perangkat Windows. Nantinya saat ada perangkat Client yang terhubung ke server HTTP (server khusus untuk tampilan web saja), perangkat tersebut akan lanjut membuat koneksi ke Server Websocket untuk menerima gambar.

WebScreen mendapatkan gambar layar langsung melalui API DirectX, tidak menggunakan metode tradisional seperti BitBlt agar - menurut pandangan saya - dapat menangkap gambar secara lebih cepat dan efisien. Setelah didapatkan, WebScreen akan memperkecil dan menkrompes gambar menjadi file PNG atau JPEG (sesuai keinginan user) agar lebih cepat untuk dikirim ke perangkat client.

## TODO
1. Kirim gambar layar melalui streaming video MP4 tanpa lewat Websocket, jangan melalui file PNG atau JPEG agar lebih cepat dan tidak memakan power usage yang tinggi (untuk menencode video dapat menggunakan Windows Media Foundation).
2. Update UI versi Client dan migrasikan ke React.

## NOTICE
Aplikasi ini tidak akan dibuat cross-platform. Khusus untuk Windows saja. Apabila ingin mencari aplikasi dengan fungsionalitas serupa di Linux, coba [Weylus](https://github.com/H-M-H/Weylus).

## Screenshot
Tampilan utama WebScreen.

![image](https://github.com/fadhil3310/WebScreen/assets/80736446/bfaa02df-5a15-47d8-8fe6-37390afb47cc)

Tampilan Client.

![Screenshot (751)](https://github.com/user-attachments/assets/3a3b29df-a2ec-4a3a-802a-7ef80c8b3dbb)
