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
                                    <h5 class="card-title">@cats[randomNum]</h5
                                    <p class="card-text">Size: ${cat.width} x ${cat.height}</p>
                                    <button class="btn btn-sm btn-outline-primary favorite-btn" data-cat-id="${cat.id}">
                                        ♡ Add to Favorites
                                    </button>
                                </div>
                            </div>
                        `;
                        
                        catContainer.appendChild(col);
                        
                        // Add event listener to the newly added favorite button
                        const favoriteBtn = col.querySelector('.favorite-btn');
                        if (favoriteBtn) {
                            favoriteBtn.addEventListener('click', function(e) {
                                addToFavorites(e.target.getAttribute('data-cat-id'), e.target);
                            });
                        }
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
    
    // Function to add a cat to favorites
    function addToFavorites(catId, buttonElement) {
        if (!catId) return;
        
        // Get the anti-forgery token
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        
        console.log('Adding cat to favorites:', catId);
        
        fetch('/Home/AddFavoriteCat', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: `catId=${catId}&__RequestVerificationToken=${token}`
        })
        .then(response => response.json())
        .then(data => {
            console.log('Server response:', data);
            if (data.success) {
                // Update button to show it's favorited
                buttonElement.innerHTML = '❤ Favorited';
                buttonElement.classList.remove('btn-outline-primary');
                buttonElement.classList.add('btn-primary');
                buttonElement.disabled = true;
            }
        })
        .catch(error => {
            console.error('Error adding cat to favorites:', error);
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
    
    // Add click event listeners to all initial favorite buttons
    document.querySelectorAll('.favorite-btn').forEach(button => {
        button.addEventListener('click', function(e) {
            addToFavorites(e.target.getAttribute('data-cat-id'), e.target);
        });
    });
}); 