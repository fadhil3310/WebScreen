# WebScreen

![image](https://github.com/fadhil3310/WebScreen/assets/80736446/bfaa02df-5a15-47d8-8fe6-37390afb47cc)


## Apa ini?
Sebuah aplikasi untuk mencerminkan layar Windows ke perangkat lain melalui internet lokal menggunakan web browser

Dapat digunakan untuk membantu developer untuk menguji aplikasi yang dibuat untuk dijalankan pada perangkat Windows layar sentuh menggunakan perangkat lain yang memiliki layar sentuh (seperti smartphone)

Pertama kali dibuat awalnya karena penasaran saja bagaimana Windows bekerja apabila menggunakan layar sentuh

## Bagaimana cara kerjanya?
Tidak terlalu kompleks

Saat aplikasi dijalankan, WebScreen akan menjalankan server http lokal untuk tampilan web bagi perangkat client dan juga server websocket untuk transmisi gambar layar perangkat Windows. Nantinya saat ada perangkat client yang terhubung ke server, perangkat tersebut akan membuat koneksi kembali ke server websocket untuk menerima gambar.

WebScreen mendapatkan gambar layar langsung melalui api DirectX, tidak menggunakan BitBlt agar - menurut pandangan saya - dapat menangkap gambar secara lebih cepat dan efisien. Setelah didapatkan, WebScreen akan memperkecil dan menkrompes gambar menjadi file PNG atau JPEG (sesuai keinginan user) agar lebih cepat untuk dikirim ke perangkat client.

## TODO
1. Kirim gambar layar melalui streaming video MP4 tanpa lewat websocket, jangan melalui file PNG atau JPEG agar lebih cepat dan tidak memakan power usage yang tinggi (untuk menencode video dapat menggunakan Windows Media Foundation)
