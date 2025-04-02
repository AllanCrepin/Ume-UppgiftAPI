// Infinite scrolling for cat images
document.addEventListener('DOMContentLoaded', function () {
    let isLoading = false;
    let currentPage = parseInt(document.getElementById('current-page').value) || 0;
    const catContainer = document.getElementById('cat-container');
    const loadingSpinner = document.getElementById('loading-spinner');

    // Function to load more cats
    function loadMoreCats() {
        if (isLoading) return;
        
        isLoading = true;
        currentPage++;
        
        // Show loading spinner
        loadingSpinner.classList.remove('d-none');
        
        // Fetch more cat images
        fetch(`/Home/LoadMoreCats?page=${currentPage}`)
            .then(response => response.json())
            .then(cats => {
                if (cats && cats.length > 0) {
                    // Add cat images to the container
                    cats.forEach(cat => {
                        const col = document.createElement('div');
                        col.className = 'col-md-3 mb-4';
                        
                        col.innerHTML = `
                            <div class="card h-100 cat-card">
                                <img src="${cat.url}" class="card-img-top" alt="Cat Image" loading="lazy">
                                <div class="card-body">
                                    <h5 class="card-title">Cat ID: ${cat.id}</h5>
                                    <p class="card-text">Size: ${cat.width} x ${cat.height}</p>
                                </div>
                            </div>
                        `;
                        
                        catContainer.appendChild(col);
                    });
                    
                    // Update hidden input with current page
                    document.getElementById('current-page').value = currentPage;
                }
                
                // Hide loading spinner
                loadingSpinner.classList.add('d-none');
                isLoading = false;
            })
            .catch(error => {
                console.error('Error loading more cats:', error);
                loadingSpinner.classList.add('d-none');
                isLoading = false;
            });
    }

    // Detect when user scrolls to bottom of page
    window.addEventListener('scroll', function() {
        const scrollHeight = document.documentElement.scrollHeight;
        const scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
        const clientHeight = document.documentElement.clientHeight;
        
        // If scrolled near bottom, load more cats
        if (scrollTop + clientHeight >= scrollHeight - 300 && !isLoading) {
            loadMoreCats();
        }
    });
}); 