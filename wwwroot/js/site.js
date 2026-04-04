// ══════════════════════════════════════
// Pasear por Pasear — Main JS
// ══════════════════════════════════════

// Mobile nav toggle
function toggleNav() {
    document.getElementById('navLinks')?.classList.toggle('open');
    document.getElementById('hamburger')?.classList.toggle('open');
}
function closeNav() {
    document.getElementById('navLinks')?.classList.remove('open');
    document.getElementById('hamburger')?.classList.remove('open');
}

// Close nav on outside click
document.addEventListener('click', (e) => {
    const nav = document.querySelector('.navbar');
    if (nav && !nav.contains(e.target)) closeNav();
});

// Scroll reveal
document.addEventListener('DOMContentLoaded', () => {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.08, rootMargin: '0px 0px -40px 0px' });

    document.querySelectorAll('.reveal').forEach(el => observer.observe(el));
});

// Language switch
function setLanguage(culture) {
    const form = document.createElement('form');
    form.method = 'POST';
    form.action = '/Language/SetLanguage';

    const cultureInput = document.createElement('input');
    cultureInput.type = 'hidden';
    cultureInput.name = 'culture';
    cultureInput.value = culture;
    form.appendChild(cultureInput);

    const returnInput = document.createElement('input');
    returnInput.type = 'hidden';
    returnInput.name = 'returnUrl';
    returnInput.value = window.location.pathname + window.location.search;
    form.appendChild(returnInput);

    // Anti-forgery token
    const token = document.querySelector('input[name="__RequestVerificationToken"]');
    if (token) {
        const tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = token.value;
        form.appendChild(tokenInput);
    }

    document.body.appendChild(form);
    form.submit();
}

// Simple rich text editor helpers
function execCmd(command, value = null) {
    document.execCommand(command, false, value);
}

function insertLink() {
    const url = prompt('URL:');
    if (url) execCmd('createLink', url);
}

// Sync contenteditable div to hidden textarea
function syncEditor(editorId, textareaId) {
    const editor = document.getElementById(editorId);
    const textarea = document.getElementById(textareaId);
    if (editor && textarea) {
        textarea.value = editor.innerHTML;
    }
}

// Initialize editors on page
document.addEventListener('DOMContentLoaded', () => {
    // Load content from hidden textareas into editors
    document.querySelectorAll('[data-editor-for]').forEach(editor => {
        const textareaId = editor.dataset.editorFor;
        const textarea = document.getElementById(textareaId);
        if (textarea) {
            editor.innerHTML = textarea.value;
            // Live sync on every keystroke
            editor.addEventListener('input', () => {
                textarea.value = editor.innerHTML;
            });
            editor.addEventListener('blur', () => {
                textarea.value = editor.innerHTML;
            });
            // Also sync on paste
            editor.addEventListener('paste', () => {
                setTimeout(() => { textarea.value = editor.innerHTML; }, 50);
            });
        }
    });

    // Intercept ALL form submissions to sync editors BEFORE submit
    document.addEventListener('submit', (e) => {
        const form = e.target;
        if (!form || form.tagName !== 'FORM') return;
        
        // Sync all editors within this form (or on the page)
        document.querySelectorAll('[data-editor-for]').forEach(editor => {
            const textareaId = editor.dataset.editorFor;
            const textarea = document.getElementById(textareaId);
            if (textarea) {
                // If editor is empty or just has <br>, set a space to avoid empty validation
                const content = editor.innerHTML.trim();
                textarea.value = (content === '' || content === '<br>') ? '' : content;
            }
        });
    }, true); // Use capture phase to run BEFORE the form submits
});

// Image preview on file input change
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('input[type="file"][data-preview]').forEach(input => {
        input.addEventListener('change', (e) => {
            const previewId = input.dataset.preview;
            const preview = document.getElementById(previewId);
            if (preview && e.target.files[0]) {
                const reader = new FileReader();
                reader.onload = (ev) => {
                    preview.src = ev.target.result;
                    preview.style.display = 'block';
                };
                reader.readAsDataURL(e.target.files[0]);
            }
        });
    });
});

// Confirm delete
function confirmDelete(formId, message) {
    if (confirm(message || '¿Estás seguro de que querés eliminar esto?')) {
        document.getElementById(formId).submit();
    }
}
