


// IMPORTANTE:
// Estos scripts son para evitar postbacks, así no se sobreescribe accidentalmente nada en la Base de Datos

(function () {
    var searchInput = document.getElementById("searchInput");
    var searchButton = document.getElementById("searchButton");
    var searchScope = document.getElementById("lista-container");
    var searchNav = document.getElementById("searchNav");
    var searchPrev = document.getElementById("searchPrev");
    var searchNext = document.getElementById("searchNext");
    var currentMatches = [];
    var currentIndex = 0;

    function clearHighlights() {
        var highlights = searchScope.querySelectorAll(".search-highlight");
        highlights.forEach(function (highlight) {
            var textNode = document.createTextNode(highlight.textContent);
            highlight.parentNode.replaceChild(textNode, highlight);
        });
        var inputHighlights = searchScope.querySelectorAll(".search-highlight-input");
        inputHighlights.forEach(function (input) {
            input.classList.remove("search-highlight-input");
        });
        var currentHighlights = searchScope.querySelectorAll(".search-highlight-current");
        currentHighlights.forEach(function (element) {
            element.classList.remove("search-highlight-current");
        });
        searchScope.normalize();
        currentMatches = [];
        currentIndex = 0;
        updateNavVisibility();
    }

    function updateNavVisibility() {
        if (searchNav) {
            searchNav.style.display = currentMatches.length ? "flex" : "none";
        }
    }

    function setCurrentMatch(index) {
        if (!currentMatches.length) {
            updateNavVisibility();
            return;
        }

        if (currentMatches[currentIndex]) {
            currentMatches[currentIndex].classList.remove("search-highlight-current");
        }

        currentIndex = (index + currentMatches.length) % currentMatches.length;
        var current = currentMatches[currentIndex];
        current.classList.add("search-highlight-current");
        current.scrollIntoView({ behavior: "smooth", block: "center" });
        updateNavVisibility();
    }

    function highlightMatches(query) {
        if (!query) {
            return [];
        }

        var matches = [];
        var walker = document.createTreeWalker(searchScope, NodeFilter.SHOW_TEXT, {
            acceptNode: function (node) {
                if (!node.nodeValue || !node.nodeValue.trim()) {
                    return NodeFilter.FILTER_REJECT;
                }
                return NodeFilter.FILTER_ACCEPT;
            }
        });

        var nodesToProcess = [];
        while (walker.nextNode()) {
            nodesToProcess.push(walker.currentNode);
        }

        nodesToProcess.forEach(function (textNode) {
            var nodeText = textNode.nodeValue;
            var regex = new RegExp(query.replace(/[.*+?^${}()|[\]\\]/g, "\\$&"), "gi");
            var match;
            var lastIndex = 0;
            var fragment = document.createDocumentFragment();

            while ((match = regex.exec(nodeText)) !== null) {
                if (match.index > lastIndex) {
                    fragment.appendChild(document.createTextNode(nodeText.slice(lastIndex, match.index)));
                }

                var highlight = document.createElement("span");
                highlight.className = "search-highlight";
                highlight.textContent = match[0];
                fragment.appendChild(highlight);
                matches.push(highlight);
                lastIndex = match.index + match[0].length;
            }

            if (matches.length && lastIndex < nodeText.length) {
                fragment.appendChild(document.createTextNode(nodeText.slice(lastIndex)));
            }

            if (fragment.childNodes.length) {
                textNode.parentNode.replaceChild(fragment, textNode);
            }
        });

        var inputs = searchScope.querySelectorAll("input[type='text'], textarea");
        var inputRegex = new RegExp(query.replace(/[.*+?^${}()|[\]\\]/g, "\\$&"), "i");
        inputs.forEach(function (input) {
            if (input.value && inputRegex.test(input.value)) {
                input.classList.add("search-highlight-input");
                matches.push(input);
            }
        });

        return matches;
    }

    function runSearch() {
        clearHighlights();
        var query = searchInput.value.trim();
        if (!query) {
            currentMatches = [];
            updateNavVisibility();
            return;
        }

        currentMatches = highlightMatches(query);
        if (!currentMatches.length) {
            updateNavVisibility();
            alert("No hay coincidencias con su búsqueda");
            return;
        }

        setCurrentMatch(0);
    }

    if (searchButton && searchInput && searchScope) {
        searchButton.addEventListener("click", runSearch);
        searchInput.addEventListener("keydown", function (event) {
            if (event.key === "Enter") {
                event.preventDefault();
                runSearch();
            }
        });
    }

    if (searchPrev) {
        searchPrev.addEventListener("click", function () {
            if (currentMatches.length) {
                setCurrentMatch(currentIndex - 1);
            }
        });
    }

    if (searchNext) {
        searchNext.addEventListener("click", function () {
            if (currentMatches.length) {
                setCurrentMatch(currentIndex + 1);
            }
        });
    }
})();
