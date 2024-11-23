// Oturum kontrolü yapan fonksiyon
function checkSessionAndRedirect(actionCallback) {
    fetch('/Account/IsSessionActive', {
        method: 'GET',
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    })
        .then(response => {
            if (response.status === 200) {
                // Oturum açık, istenen işlemi gerçekleştir
                actionCallback();
            } else if (response.status === 401) {
                // Oturum kapalı, login sayfasına yönlendir
                alert("Bir işlem yapmak için lütfen oturum açınız!");
                window.location.href = '/Account/Login';
            }
        })
        .catch(error => console.error('Oturum kontrol hatası:', error));
}

const searchButton = document.getElementById('searchButton');
if (searchButton) {
    searchButton.addEventListener('click', function () {
        let query = document.getElementById('searchInput').value.trim();

        if (query === '') {
            document.querySelector('#productTable tbody').innerHTML = '';
            document.getElementById('warning').textContent = ''; // Uyarıyı gizle
            document.getElementById('dropdown').style.display = 'none'; // Tabloyu gizle
            return;
        }

        // En az 3 karakter girildi mi kontrol et
        if (query.length < 3) {
            document.getElementById('warning').textContent = 'En az 3 karakter giriniz.';
            document.querySelector('#productTable tbody').innerHTML = ''; // Sonuçları temizle
            document.getElementById('dropdown').style.display = 'none'; // Tabloyu gizle
            return;
        }

        // 3 karakter girildiyse uyarıyı kaldır ve aramayı yap
        document.getElementById('warning').textContent = '';

        // Arama fonksiyonunu çağır
        searchProducts(query);
    });
} else {
    console.error("searchButton öğesi bulunamadı!");
}



// Arama fonksiyonu
function searchProducts(query) {
    console.log('Arama yapılıyor:', query);

    // Fetch veya diğer işlemlerle arama yapabilirsiniz
    fetch(`/Home/Search?query=${query}`)
        .then(response => response.json())
        .then(data => {
            let tableBody = document.querySelector('#productTable tbody');
            tableBody.innerHTML = ''; // Önceki sonuçları temizle

            // Eğer veri geldiyse tabloyu göster
            if (data.$values.length > 0) {
                document.getElementById('dropdown').style.display = 'block';
            } else {
                document.getElementById('dropdown').style.display = 'none';
            }
            console.log(data)
            // Gelen verilerden her birini tabloya satır olarak ekle
            if (data.$values && data.$values.length > 0) {
                data.$values.forEach(item => {
                    let row = document.createElement('tr');
                    let shortenedDescription = item.description.length > 30 ? item.description.substring(0, 30) + '...' : item.description;

                    row.innerHTML = `
                    <td>${item.brand.name}</td>
                    <td>${item.productCode}</td>
                    <td class="tooltip-text" data-tooltip="${item.description}" title="${item.description}">${shortenedDescription}</td>
                    <td>${item.listPrice} ${item.currency}</td>
                    <td>
                        <div style="display: flex; align-items: center;">
                            <input type="number" class="form-control input-small text-right quantity" name="item.quantity" min="1" max="999999" value="0" maxlength="6" />
                            <select class="form-control unit-select" name="unit" style="margin-left: 5px;">
                                <option value="adet">Adet</option>
                                <option value="m">Metre</option>
                                <option value="kg">Kilogram</option>
                            </select>
                        </div>
                    </td>
                    <td><input type="number" class="form-control input-small text-right discount" name="discount" min="0" value="0" /></td>
                    <td><input type="date" class="form-control text-right delivery-time" name="deliveryTime" /></td>
                    <td>
                        <div style="display: flex; align-items: center;">
                            <input type="text" class="form-control text-center total-price" placeholder="Toplam" readonly />
                            <button class="btn btn-success calculate-button" style="margin-left: 5px;">Hesapla</button>
                        </div>
                    </td>
                    <td class="text-center">
                        <button type="submit" class="btn btn-primary add-to-wishlist">
                            <i class="fa-solid fa-plus fa-beat"></i>
                        </button>
                    </td>
                    <td class="text-center">
                        <button class="btn btn-info email-share" style="color:white">
                            <i class="fa-regular fa-envelope fa-beat"></i>
                        </button>
                    </td>
                `;

                    tableBody.appendChild(row);

                    // Event Listener'lar
                    row.querySelector('.quantity').addEventListener('input', function () {
                        if (this.value.length > 6) {
                            this.value = this.value.slice(0, 6);
                        }
                    });

                    const deliveryTimeInput = row.querySelector('.delivery-time');
                    const today = new Date().toISOString().split("T")[0];
                    deliveryTimeInput.setAttribute("min", today);

                    row.querySelector('.calculate-button').addEventListener('click', function () {
                        checkSessionAndRedirect(() => {
                            totalPrice(row, item.listPrice, item.currency);
                            console.log('Hesaplama işlemi yapılıyor...');
                        });
                    });

                    row.querySelector('.add-to-wishlist').addEventListener('click', function () {
                        checkSessionAndRedirect(() => {
                            item.quantity = row.querySelector('.quantity').value;
                            item.discount = row.querySelector('.discount').value;
                            item.deliveryTime = row.querySelector('.delivery-time').value;
                            item.total = row.querySelector('.total-price').value;

                            let total = row.querySelector('.quantity').value * item.listPrice * (1 - item.discount / 100);

                            if (item.total === "" && row.querySelector('.quantity').value > 0 && item.deliveryTime !== "") {
                                totalPrice(row, item.listPrice, item.currency);
                            }
                            item.total = row.querySelector('.total-price').value;

                            addToWishlist(item);
                            console.log('İstek listesine ürün ekleniyor...');
                        });
                    });

                    row.querySelector('.email-share').addEventListener('click', function () {
                        checkSessionAndRedirect(() => {
                            sendEmail(item);
                            console.log('Email gönderme işlemi...');
                        });
                    });
                });
            } else {
                console.log('Hiçbir sonuç bulunamadı.');
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });
}




// İstek listesine ürün ekleyen fonksiyon
function addToWishlist(item) {
    if (item.listPrice == 0 || item.quantity == 0 || item.deliveryTime == "" || item.discount < 0) {
        alert("Lütfen bütün alanları doldurunuz!");
        return;
    }

    let wishlist = document.querySelector('#wishlistItems tbody'); // Tablo gövdesi

    // Eğer tablo boşsa istek listesini tekrar görünür hale getir
    if (wishlist.children.length === 0) {
        document.getElementById('wishlist').style.display = 'block'; // İstek listesini göster
    }

    // Yeni bir satır oluştur
    let row = document.createElement('tr');
    let shortenedDescription = item.description.length > 30 ? item.description.substring(0, 30) + '...' : item.description;
    row.innerHTML = `
        <td class="text-center">${item.brand.name}</td>
        <td class="text-center">${item.productCode}</td>
        <td class="text-center tooltip-text" data-tooltip="${item.description}" title="${item.description}">${shortenedDescription}</td>
        <td class="text-center">${item.formattedPrice}</td>
        <td class="text-center">${item.quantity + " " + item.currency}</td>
        <td class="text-center">${item.discount}%</td>
        <td class="text-center">${calculateDateDifference(item.deliveryTime)}</td>
        <td class="text-center">${item.total}</td>
        <td class="text-center">
            <a href="${item.link}" target='_blank'>
                <i class="fa-solid fa-arrow-up-right-from-square fa-lg"></i>
            </a>
        </td>
        <td class="text-center">
            <button class="btn btn-danger trash-icon" style="color:white">
                <i class="fa-solid fa-trash fa-beat"></i>
            </button>
        </td>
    `;

    // Çöp kutusu simgesine tıklanınca ürünü istek listesinden kaldır
    row.querySelector('.trash-icon').addEventListener('click', function () {
        row.remove();

        // Eğer istek listesi boşsa tabloyu gizle
        if (wishlist.children.length === 0) {
            document.getElementById('wishlist').style.display = 'none'; // İstek listesini gizle
        }
    });

    wishlist.appendChild(row); // Satırı tabloya ekle

    // Toast bildirimi
    toastr.success('', 'Teklif Eklendi', {
        timeOut: 3000, // 3 saniye sonra kaybolacak
        positionClass: "toast-bottom-right"
    });
}

// Yeni satır eklemek için bir işlev tanımlıyoruz
function addEmptyRow() {
    let wishlist = document.querySelector('#wishlistItems tbody');

    let emptyRow = document.createElement('tr');
    emptyRow.innerHTML = `
        <td><input type="text" class="form-control text-center" placeholder="Marka"/></td>
        <td><input type="text" class="form-control text-center" placeholder="Ürün Kodu"/></td>
        <td><input type="text" class="form-control text-center" placeholder="Açıklama"/></td>
         <td> <div style="display: flex; align-items: center;">
                <input type="number" class="form-control text-center list-price" placeholder="Fiyat"/>
                <select class="form-control currency" name="currency" style="margin-left: 5px; appearance: auto;">
                    <option value="₺">TL</option>
                    <option value="$">USD</option>
                    <option value="€">EUR</option>
                </select>
            </div></td>
      <td>
    <div style="display: flex;">
        <input type="number" class="form-control text-right quantity" name="item.quantity" min="1" max="999999" value="0" style="flex: 3; max-width: 100px;" />
        <select class="form-control unit-select" name="unit" style="flex: 1; max-width: 80px; margin-left: 5px;">
            <option value="adet">Adet</option>
            <option value="m">Metre</option>
            <option value="kg">Kilogram</option>
        </select>
    </div>
</td>
        <td><input type="number" class="form-control text-center discount" min="1" max="100" value="0" placeholder="İndirim (%)"/></td>
       <td>
    <input type="date" class="form-control text-center delivery-time" onchange="calculateDateDifferenceEmptyRow(this)" name="deliveryTime" min=""/>
    <input type="text" class="form-control text-center date-difference" style="display:none" readonly />
</td>

         <td>
            <div style="display: flex; align-items: center;">
            <input type="text" class="form-control text-center total-price" placeholder="Toplam" readonly/>
                <button class="btn btn-success calculate-button" style="margin-left: 5px;">Hesapla</button>
            </div>
        </td>
        <td class="text-center">
            <a href="#" target='_blank'>
                <i class="fa-solid fa-arrow-up-right-from-square fa-lg"></i>
            </a>
        </td>
        <td class="text-center">
            <button class="btn btn-danger trash-icon" style="color:white">
                <i class="fa-solid fa-trash fa-beat"></i>
            </button>
        </td>
    `;

    // Çöp kutusu simgesine tıklanınca ürünü istek listesinden kaldır
    emptyRow.querySelector('.trash-icon').addEventListener('click', function () {
        emptyRow.remove();

        // Eğer istek listesi boşsa tabloyu gizle
        if (wishlist.children.length === 0) {
            document.getElementById('wishlist').style.display = 'none'; // İstek listesini gizle
        }
    });

    // Hesapla butonuna tıklama olay işleyicisi ekle
    emptyRow.querySelector('.calculate-button').addEventListener('click', function () {

        // Satırdaki listPrice değerini al
        let listPrice = parseFloat(emptyRow.querySelector('.list-price').value) || 0;
        let currency = emptyRow.querySelector('.currency').value;

        // totalPrice fonksiyonunu çağırarak toplam fiyatı hesapla
        totalPrice(emptyRow, listPrice, currency);
    });

    wishlist.appendChild(emptyRow);

    // Eğer istek listesi görünmüyorsa tabloyu göster
    if (wishlist.children.length > 0) {
        document.getElementById('wishlist').style.display = 'block';
    }
}


// Yeni satır ekleme butonunu oluşturuyoruz
let addRowButton = document.createElement('button');
addRowButton.textContent = "Boş Satır Ekle";
addRowButton.className = "btn btn-secondary";
addRowButton.style.marginTop = "10px";

// Yeni satır ekleme butonuna tıklama olayı ekliyoruz
addRowButton.addEventListener('click', addEmptyRow);

// Wishlist tablosunun altına butonu ekliyoruz
document.getElementById('wishlist').appendChild(addRowButton);



// Email gönderme fonksiyonu
function sendEmail(item) {
    if (item.listPrice == 0 || item.quantity == 0 || item.deliveryTime == "" || item.discount < 0) {
        alert("Lütfen bütün alanları doldurunuz!");
        return;
    }
    let subject = 'Ürün Bilgisi: ' + item.productCode;
    let body = `
        Marka: ${item.brand}\n
        Ürün Kodu: ${item.productCode}\n
        Açıklama: ${item.description}\n
        Fiyat: ${item.listPrice}\n
        Adet: ${item.quantity}\n
        İskonto: ${item.discount}\n
        Teslim Süresi: ${item.deliveryTime}\n
        Toplam Bedel: ${item.total}
    `;
    window.location.href = `mailto:?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`;
}

function totalPrice(row, listPrice, currency) {
    // Satırdaki quantity ve discount input'larını bul
    let quantityInput = row.querySelector('.quantity');
    let discountInput = row.querySelector('.discount');
    let totalPriceInput = row.querySelector('.total-price');
    let deliveryTimeInput = row.querySelector('.delivery-time');

    if (listPrice == 0 || quantityInput.value == 0 || deliveryTimeInput.value == "" || discountInput.value < 0) {
        alert("Lütfen bütün alanları doldurunuz!");
        return;
    }



    // Kullanıcıdan gelen quantity ve discount değerlerini al
    let quantity = parseFloat(quantityInput.value) || 1;  // Varsayılan olarak 1
    let discount = parseFloat(discountInput.value) / 100 || 0;  // İskonto yüzdesi 0'dan 100'e kadar olmalı

    // Toplam fiyatı hesapla
    let total = listPrice * quantity * (1 - discount);

    // Hesaplanan toplam fiyatı input'a yaz
    totalPriceInput.value = total.toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + " " + currency;
}


function calculateDateDifference(selectedDate) {
    // Bugünün tarihini alın
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Saatleri sıfırlayarak sadece tarih farkına odaklanın

    // Seçilen tarihi alın ve tarih objesine dönüştürün
    const targetDate = new Date(selectedDate);
    targetDate.setHours(0, 0, 0, 0);

    // Tarihler arasındaki farkı hesaplayın (milisaniye cinsinden)
    const diffTime = targetDate - today;
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)); // Milisaniyeden güne çevirin

    if (diffDays <= 6) {
        // 6 gün ve daha az için "X Gün" formatında
        return `${diffDays} Gün`;
    } else {
        // 7 gün ve daha fazla için "X-X Hafta" formatında
        const weeks = Math.floor(diffDays / 7);
        const remainingDays = diffDays % 7;

        // "X Hafta" veya "X-X Hafta" olarak döndür
        return remainingDays === 0 ? `${weeks} Hafta` : `${weeks}-${weeks + 1} Hafta`;
    }
}

function calculateDateDifferenceEmptyRow(input) {
    // Bugünün tarihini alın
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Saatleri sıfırlayarak sadece tarih farkına odaklanın

    // Seçilen tarihi alın ve tarih objesine dönüştürün
    const selectedDate = input.value;
    const targetDate = new Date(selectedDate);
    targetDate.setHours(0, 0, 0, 0);

    // Tarihler arasındaki farkı hesaplayın (milisaniye cinsinden)
    const diffTime = targetDate - today;
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)); // Milisaniyeden güne çevirin

    let result;
    if (diffDays <= 6) {
        // 6 gün ve daha az için "X Gün" formatında
        result = `${diffDays} Gün`;
    } else {
        // 7 gün ve daha fazla için "X-X Hafta" formatında
        const weeks = Math.floor(diffDays / 7);
        const remainingDays = diffDays % 7;

        // "X Hafta" veya "X-X Hafta" olarak döndür
        result = remainingDays === 0 ? `${weeks} Hafta` : `${weeks}-${weeks + 1} Hafta`;
    }
    input.style.display = 'none';
    // Hesaplanan değeri yanındaki <span> etiketine yaz
    const dateDifferenceSpan = input.parentElement.querySelector('.date-difference');
    dateDifferenceSpan.value = result;
    dateDifferenceSpan.style.display = 'block'
}