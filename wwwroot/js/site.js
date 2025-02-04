// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Проверяем наличие токена при загрузке страницы
window.onload = function() {
    const token = localStorage.getItem('jwt_token');
    if (token) {
        document.getElementById('loginItem').style.display = 'none';
        document.getElementById('profileItem').style.display = 'block';
        
        // Декодируем JWT токен для получения имени пользователя
        const payload = JSON.parse(atob(token.split('.')[1]));
        const userName = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
        document.getElementById('userName').textContent = userName;
    } else {
        document.getElementById('loginItem').style.display = 'block';
        document.getElementById('profileItem').style.display = 'none';
    }
    updateCartCount();
    updateUserBalance();
}

// Функция выхода теперь будет в личном кабинете
function logout() {
    if (confirm('Вы уверены, что хотите выйти?')) {
        localStorage.removeItem('jwt_token');
        window.location.href = '/Home/Login';
    }
}

function updateQuantity(productId, delta) {
    const token = localStorage.getItem('jwt_token');
    fetch('/Cart/UpdateQuantity', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Authorization': token ? `Bearer ${token}` : ''
        },
        body: JSON.stringify({ 
            productId: productId, 
            delta: delta 
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Обновляем страницу для отображения новых данных
            window.location.reload();
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Ошибка при обновлении количества товара');
    });
}

let searchTimeout;

document.addEventListener('DOMContentLoaded', function() {
    const searchInput = document.querySelector('input[name="query"]');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                if (this.value.length >= 3) {
                    this.closest('form').submit();
                }
            }, 500);
        });
    }
});

function updateCartCount() {
    fetch('/Cart/GetCartCount')
        .then(response => response.json())
        .then(data => {
            const cartCountElement = document.getElementById('cartCount');
            if (cartCountElement) {
                cartCountElement.textContent = data.count || 0;
            }
        })
        .catch(error => {
            console.error('Error updating cart count:', error);
        });
}

function updateUserBalance() {
    fetch('/Balance/GetBalance')
        .then(response => response.json())
        .then(data => {
            const balanceElement = document.getElementById('userBalance');
            if (balanceElement) {
                balanceElement.textContent = new Intl.NumberFormat('ru-RU').format(data.balance);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            toastr.error('Ошибка при обновлении баланса');
        });
}

async function addToCart(productId) {
    try {
        const response = await fetch('/Cart/AddToCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ productId: productId })
        });

        if (response.status === 401) {
            // Если пользователь не авторизован, перенаправляем на страницу входа
            window.location.href = '/Home/Login';
            return;
        }

        // ... остальной код обработки ответа ...
    } catch (error) {
        console.error('Error:', error);
        toastr.error('Произошла ошибка при добавлении товара в корзину');
    }
}

async function addToWishlist(productId) {
    try {
        const response = await fetch('/Wishlist/AddToWishlist', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(productId)
        });

        if (response.status === 401) {
            // Если пользователь не авторизован, перенаправляем на страницу входа
            window.location.href = '/Home/Login';
            return;
        }

        // ... остальной код обработки ответа ...
    } catch (error) {
        console.error('Error:', error);
        toastr.error('Произошла ошибка при добавлении товара в список избранного');
    }
}
