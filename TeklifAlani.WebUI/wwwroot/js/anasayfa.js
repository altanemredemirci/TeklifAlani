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

            // Gelen verilerden her birini tabloya satır olarak ekle
            if (data.$values && data.$values.length > 0) {
                data.$values.forEach(item => {
                    let row = document.createElement('tr');
                    let shortenedDescription = item.description.length > 30 ? item.description.substring(0, 30) + '...' : item.description;

                    // İlk üç sütunu ekle
                    let rowContent = `
            <td>${item.brand.name}</td>
            <td>${item.productCode}</td>
            <td class="tooltip-text" data-tooltip="${item.description}" title="${item.description}">${shortenedDescription}</td>
        `;

                    // Koşullu olarak fiyat sütununu ekle
                    if (item.listPrice === 0) {
                        rowContent += `
                     <td>
                         <div style="align-items: center;">
                             <input type="number" class="form-control text-center list-price" placeholder="Fiyat" />
                             <select class="form-control currency" name="currency" style="appearance: auto;">
                                 <option value="₺">TL</option>
                                 <option value="$">USD</option>
                                 <option value="€">EUR</option>
                             </select>
                         </div>
                     </td>`

                            ;                        
                    } else {
                        rowContent += `
            <td>${item.listPrice} ${item.currency}</td>`;
                    }

                    // Geri kalan sütunları ekle
                    rowContent += `
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
            <td><input type="number" class="form-control input-small text-right discount" name="discount" min="0" value="0" max="100" value="0" maxlength="3"  /></td>
            <td><input type="date" class="form-control text-right delivery-time" name="deliveryTime" /></td>
            <td>
                <div style="display: flex; align-items: center;">
                    <input type="text" class="form-control text-center total-price" placeholder="Toplam" readonly />
                    <button class="btn btn-success calculate-button" style="margin-left: 5px;">Hesapla</button>
                </div>
            </td>
            <td class="text-center" style="vertical-align: middle;">
                <button type="submit" class="btn btn-primary add-to-wishlist">
                    <i class="fa-solid fa-plus fa-beat"></i>
                </button>
                <button class="btn btn-info email-share" style="color:white">
                    <i class="fa-regular fa-envelope fa-beat"></i>
                </button>
            </td>
        `;



                    // Satırı HTML içeriğiyle doldur ve tabloya ekle
                    row.innerHTML = rowContent;
                    tableBody.appendChild(row);
                    // Liste fiyatı için event listener
                    const listPrice = row.querySelector('.list-price');
                    if (listPrice) {
                        listPrice.addEventListener('input', function () {
                            const value = parseFloat(this.value); // Kullanıcının girdiği değeri al
                            if (!isNaN(value)) {
                                item.listPrice = value; // Değeri item.listPrice'a ata
                                item.currency = row.querySelector('.currency').value;                               
                                console.log('List Price updated:', item.listPrice);
                            } else {
                                console.log('Invalid input for list price');
                            }
                        });
                    }
                    // Event Listener'ları ekle
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
                            if (item.currency == null) {
                                item.currency = row.querySelector('.currency').value;
                            }
                            totalPrice(row, item.listPrice, item.currency);
                            console.log('Hesaplama işlemi yapılıyor...');
                        });
                    });

                    row.querySelector('.add-to-wishlist').addEventListener('click', function () {
                        checkSessionAndRedirect(() => {
                            if (item.currency == null) {
                                item.currency = row.querySelector('.currency').value;
                            }    
                            item.quantity = row.querySelector('.quantity').value;
                            item.discount = row.querySelector('.discount').value;
                            item.unit = row.querySelector('.unit-select').value;
                            item.deliveryTime = row.querySelector('.delivery-time').value;
                                                   
                            totalPrice(row, item.listPrice, item.currency);

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
}

var totalLira = 0;
var totalDolar = 0;
var totalEuro = 0;

// İstek listesine ürün ekleyen fonksiyon
function addToWishlist(item) {
    if (item.listPrice === 0 || item.quantity === 0 || item.deliveryTime === "" || item.discount < 0) {
        alert("Lütfen bütün alanları doldurunuz!");
        return;
    }

    // Toplam bedelin formatını düzelt ve topla
    let totalValue = parseFloat(item.total.replace(/[^0-9.,]/g, '').replace(".","").replace(',', '.'));   
    if (item.currency === "₺") totalLira += totalValue;
    else if (item.currency === "$") totalDolar += totalValue;
    else if (item.currency === "€") totalEuro += totalValue;

    // Yeni satır ekleme işlemi

    let shortenedDescription = item.description.length > 30 ? item.description.substring(0, 30) + '...' : item.description;
    let wishlist = document.querySelector('#wishlistItems tbody');
    let row = document.createElement('tr');
    row.innerHTML = `
        <td class="text-center">${item.brand.name}</td>
        <td class="text-center">${item.productCode}</td>
       <td class="tooltip-text" data-tooltip="${item.description}" title="${item.description}">${shortenedDescription}</td>
        <td class="text-center">${item.listPrice} ${item.currency}</td>
        <td class="text-center">${item.quantity} ${item.unit}</td>
        <td class="text-center">${item.discount}%</td>
        <td class="text-center">${calculateDateDifferenceGeneric(item.deliveryTime)}</td>
        <td class="text-center">${item.total}</td>
        <td class="text-center">
            <button class="btn btn-danger trash-icon" style="color:white">
                <i class="fa-solid fa-trash fa-beat"></i>
            </button>
        </td>


   

    `;

    // Çöp kutusu butonu
    row.querySelector('.trash-icon').addEventListener('click', function () {
        row.remove();
        removeEmptyRow(item);
      
    });

    wishlist.appendChild(row);
    // Toplam değerleri güncelle
    document.querySelector('#totalLira').textContent = `${totalLira} ₺`;
    document.querySelector('#totalDolar').textContent = `${totalDolar} $`;
    document.querySelector('#totalEuro').textContent = `${totalEuro} €`;
    
    document.getElementById("wishlist").style.display = 'block';
    // Başarı mesajı
    toastr.success('', 'Teklif Eklendi', { timeOut: 3000, positionClass: "toast-bottom-right" });
}


// Yeni satır eklemek için bir işlev tanımlıyoruz
function addEmptyRow() {
    document.getElementById('dropdown').style.display = 'block';
    let productTable = document.querySelector('#productTable tbody');

    let emptyRow = document.createElement('tr');

    // Yeni bir geçici item nesnesi oluştur
    let item = {
        brand: "",
        productCode: "",
        description: "",
        listPrice: 0,
        currency: "₺",
        quantity: 0,
        discount: 0,
        deliveryTime: "",
        total: 0
    };

    emptyRow.innerHTML = `
        <td><input type="text" class="form-control text-center" placeholder="Marka"/></td>
        <td><input type="text" class="form-control text-center" placeholder="Ürün Kodu"/></td>
        <td><input type="text" class="form-control text-center" placeholder="Açıklama"/></td>
        <td>
                         <div style="align-items: center;">
                             <input type="number" class="form-control text-center list-price" placeholder="Fiyat" />
                             <select class="form-control currency" name="currency" style="appearance: auto;">
                                 <option value="₺">TL</option>
                                 <option value="$">USD</option>
                                 <option value="€">EUR</option>
                             </select>
                         </div>
                     </td>
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
       <td><input type="number" class="form-control input-small text-right discount" name="discount" min="0" value="0" max="100" value="0" maxlength="3"  /></td>
       <td>
            <input type="date" class="form-control text-center delivery-time" onchange="calculateDateDifferenceGeneric(this)" name="deliveryTime" min=""/>
        </td>
         <td>
            <div style="display: flex; align-items: center;">
                <input type="text" class="form-control text-center total-price" placeholder="Toplam" readonly/>
                <button class="btn btn-success calculate-button" style="margin-left: 5px;">Hesapla</button>
            </div>
        </td>      
        <td class="text-center">
            <button type="submit" class="btn btn-primary add-to-wishlist">
                <i class="fa-solid fa-plus fa-beat"></i>
            </button>
            <button class="btn btn-danger trash-icon" style="color:white">
                <i class="fa-solid fa-trash fa-beat"></i>
            </button>
        </td>
    `;

    // Çöp kutusu simgesine tıklanınca ürünü istek listesinden kaldır
    emptyRow.querySelector('.trash-icon').addEventListener('click', function () {
       
       

        emptyRow.remove();
        // Toplam değerleri güncelle
        document.querySelector('#totalLira').textContent = `${totalLira} ₺`;
        document.querySelector('#totalDolar').textContent = `${totalDolar} $`;
        document.querySelector('#totalEuro').textContent = `${totalEuro} €`;
        document.getElementById("wishlist").style.display = 'block';
        // Eğer istek listesi boşsa tabloyu gizle
        if (productTable.children.length === 0) {
            document.getElementById('dropdown').style.display = 'none'; // İstek listesini gizle
        }
    });

    emptyRow.querySelector('.add-to-wishlist').addEventListener('click', function () {
        const item = {
            brand: { name: emptyRow.querySelector('input[placeholder="Marka"]').value }, 
            productCode: emptyRow.querySelector('input[placeholder="Ürün Kodu"]').value,
            description: emptyRow.querySelector('input[placeholder="Açıklama"]').value,
            listPrice: parseFloat(emptyRow.querySelector('.list-price').value) || 0,
            currency: emptyRow.querySelector('.currency').value,
            quantity: parseInt(emptyRow.querySelector('.quantity').value) || 0,
            discount: parseInt(emptyRow.querySelector('.discount').value) || 0,
            unit: emptyRow.querySelector('.unit-select').value,
            deliveryTime: emptyRow.querySelector('.delivery-time').value
        };
        
        totalPrice(emptyRow, item.listPrice, item.currency);
        item.total = emptyRow.querySelector('.total-price').value;
        checkSessionAndRedirect(() => addToWishlist(item));
    });

    // Hesapla butonuna tıklama olay işleyicisi ekle
    emptyRow.querySelector('.calculate-button').addEventListener('click', function () {
        checkSessionAndRedirect(() => {
            let listPrice = parseFloat(emptyRow.querySelector('.list-price').value) || 0;
            let currency = emptyRow.querySelector('.currency').value;
            totalPrice(emptyRow, listPrice, currency);
            console.log('Hesaplama işlemi yapılıyor...');
        });
    });

    productTable.appendChild(emptyRow);
}

function removeEmptyRow(item) {
    let totalValue = parseFloat(item.total.replace(/[^0-9.,]/g, '').replace(".", "").replace(',', '.'));
    if (item.currency === "₺") totalLira -= totalValue;
    else if (item.currency === "$") totalDolar -= totalValue;
    else if (item.currency === "€") totalEuro -= totalValue;

    if (wishlist.children.length === 0) {
        document.getElementById('wishlist').style.display = 'none';
    }

    // Toplam değerleri güncelle
    document.querySelector('#totalLira').textContent = `${totalLira} ₺`;
    document.querySelector('#totalDolar').textContent = `${totalDolar} $`;
    document.querySelector('#totalEuro').textContent = `${totalEuro} €`;
} 

// Yeni satır ekleme butonunu oluşturuyoruz
let addRowButton = document.createElement('button');
addRowButton.textContent = "Boş Satır Ekle";
addRowButton.className = "btn btn-danger";
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


function calculateDateDifferenceGeneric(selectedDate) {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const targetDate = new Date(selectedDate);
    targetDate.setHours(0, 0, 0, 0);

    const diffDays = Math.ceil((targetDate - today) / (1000 * 60 * 60 * 24));
    if (diffDays <= 6) return `${diffDays} Gün`;

    const weeks = Math.floor(diffDays / 7);
    const remainingDays = diffDays % 7;
    return remainingDays === 0 ? `${weeks} Hafta` : `${weeks}-${weeks + 1} Hafta`;
}


