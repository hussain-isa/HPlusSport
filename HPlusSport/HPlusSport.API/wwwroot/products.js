const productsGrid = document.querySelector('#products-grid');
const productCount = document.querySelector('#product-count');
const statusMessage = document.querySelector('#status-message');
const refreshButton = document.querySelector('#refresh-products');
const productsEndpoint = '/api/products';

async function loadProducts() {
    setStatus('Loading products...');
    productsGrid.innerHTML = '';

    try {
        const response = await fetch(productsEndpoint);

        if (!response.ok) {
            throw new Error(`Products API returned ${response.status}`);
        }

        const products = await response.json();
        renderProducts(products);
        setStatus('');
    } catch (error) {
        productCount.textContent = 'Unable to load products';
        setStatus(error.message, true);
    }
}

function renderProducts(products) {
    productCount.textContent = `${products.length} products available`;

    productsGrid.innerHTML = products.map(product => `
        <article class="product-card">
            <span class="product-badge ${product.isAvailable ? '' : 'unavailable'}">
                ${product.isAvailable ? 'Available' : 'Unavailable'}
            </span>
            <div>
                <p class="product-sku">${escapeHtml(product.sku)}</p>
                <h3>${escapeHtml(product.name)}</h3>
            </div>
            <p class="product-meta">Category ${product.categoryId}</p>
            <p class="product-meta">${escapeHtml(product.description || 'No description provided.')}</p>
            <div class="product-price">${formatPrice(product.price)}</div>
        </article>
    `).join('');
}

function setStatus(message, isError = false) {
    statusMessage.textContent = message;
    statusMessage.style.color = isError ? '#b42318' : '#0d5e50';
}

function formatPrice(price) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(price);
}

function escapeHtml(value) {
    return String(value).replace(/[&<>'"]/g, character => ({
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        "'": '&#39;',
        '"': '&quot;'
    }[character]));
}

refreshButton.addEventListener('click', loadProducts);
loadProducts();
