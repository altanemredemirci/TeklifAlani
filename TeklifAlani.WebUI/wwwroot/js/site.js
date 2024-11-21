// Arama kutusuna girilen değeri işleyen fonksiyon
document.getElementById('searchInput').addEventListener('keyup', function () {
    let query = this.value;

    // Boşsa dropdown'ı temizle
    if (query === '') {
        document.getElementById('dropdown').innerHTML = '';
        document.getElementById('warning').textContent = ''; // Uyarıyı gizle
        document.getElementById('dropdown').style.display = 'none'; // Dropdown'ı gizle
        return;
    }

    // En az 3 karakter girildi mi kontrol et
    if (query.length < 3) {
        document.getElementById('warning').textContent = 'En az 3 karakter giriniz.';
        document.getElementById('dropdown').innerHTML = ''; // Sonuçları temizle
        document.getElementById('dropdown').style.display = 'none'; // Dropdown'ı gizle
        return;
    }

    // 3 karakter girildiyse uyarıyı kaldır ve aramayı yap
    document.getElementById('warning').textContent = '';

    // Fetch API ile arama sorgusu gönder
    fetch(`/Home/Search?query=${query}`)
        .then(response => response.json())
        .then(data => {
            let dropdown = document.getElementById('dropdown');
            dropdown.innerHTML = `<div class="row header">
                <div class="col-1 text-center">Marka</div>
                <div class="col-2 text-center">Ürün Kodu</div>
                <div class="col-2 text-center">Açıklama</div>
                <div class="col-2 text-center">Liste Fiyatı</div>
                <div class="col-1 text-center">Adet</div>
                <div class="col-1 text-center">İskonto</div>
                <div class="col-2 text-center">Teslim Süresi</div>
                <div class="col-1 text-center">Bedel</div>
            </div>`; // Başlıkları tekrar göster

            dropdown.style.display = 'block'; // Sonuç varsa dropdown'ı göster

            // Gelen verilerden yalnızca ilk 10 öğeyi listele
            data.forEach(item => {
                let div = document.createElement('div');
                div.className = 'row dropdown-item'; // Satır yapısı
                div.innerHTML = `
                    <div class="col-1 text-center">${item.brand}</div>
                    <div class="col-2 text-center">${item.productCode}</div>
                    <div class="col-2 text-center">${item.description}</div>
                    <div class="col-2 text-center">${item.price}</div>
                    <div class="col-1 text-center">${item.quantity}</div>
                    <div class="col-1 text-center">${item.discount}</div>
                    <div class="col-2 text-center">${item.deliveryTime}</div>
                    <div class="col-1 text-center">${item.total}</div>`;

                dropdown.appendChild(div); // Her öğeyi başlık altına ekle
            });
        })
        .catch(error => console.error('Error:', error));
});
