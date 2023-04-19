# WebScreen

## Apa ini?
Sebuah aplikasi untuk mencerminkan layar Windows ke perangkat lain melalui internet lokal menggunakan web browser

Dapat digunakan untuk membantu developer untuk menguji aplikasi yang dibuat untuk dijalankan pada perangkat Windows layar sentuh menggunakan perangkat lain yang memiliki layar sentuh (seperti smartphone)

Pertama kali dibuat awalnya karena penasaran saja bagaimana Windows bekerja apabila menggunakan layar sentuh

## Bagaimana cara kerjanya?
Tidak terlalu kompleks

Saat aplikasi dijalankan, WebScreen akan menjalankan server http lokal untuk tampilan web bagi perangkat lain dan juga server websocket untuk transmisi gambar layar perangkat Windows. Nantinya saat ada perangkat lain yang terhubung ke server, perangkat tersebut akan membuat koneksi ke server melalui websocket untuk menerima gambar.

Untuk mendapatkan gambar layar, WebScreen menggunakan DirectX. Setelah didapatkan, WebScreen akan memperkecil dan menkrompes gambar agar lebih cepat untuk dikirim ke perangkat lain.
