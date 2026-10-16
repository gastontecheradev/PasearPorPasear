/* ══════════════════════════════════════════════════════════════
   PASEAR POR PASEAR — comportamiento del sitio
   ══════════════════════════════════════════════════════════════ */
(function () {
  'use strict';

  // ── Menú desplegable en pantallas chicas ──────────────────────
  var ham = document.getElementById('hamburguesa');
  var nav = document.getElementById('nav');

  if (ham && nav) {
    ham.addEventListener('click', function (e) {
      e.stopPropagation();
      var abierto = nav.classList.toggle('abierta');
      ham.setAttribute('aria-expanded', abierto ? 'true' : 'false');
    });

    // Cerrar al tocar fuera
    document.addEventListener('click', function (e) {
      if (!nav.classList.contains('abierta')) return;
      if (nav.contains(e.target) || ham.contains(e.target)) return;
      nav.classList.remove('abierta');
      ham.setAttribute('aria-expanded', 'false');
    });

    // Cerrar con Escape y devolver el foco al botón
    document.addEventListener('keydown', function (e) {
      if (e.key !== 'Escape' || !nav.classList.contains('abierta')) return;
      nav.classList.remove('abierta');
      ham.setAttribute('aria-expanded', 'false');
      ham.focus();
    });
  }

  // ── Cambio de idioma ──────────────────────────────────────────
  window.setLanguage = function (culture) {
    var form = document.createElement('form');
    form.method = 'POST';
    form.action = '/Language/SetLanguage';

    function campo(nombre, valor) {
      var i = document.createElement('input');
      i.type = 'hidden';
      i.name = nombre;
      i.value = valor;
      form.appendChild(i);
    }

    campo('culture', culture);
    campo('returnUrl', window.location.pathname + window.location.search);

    var token = document.querySelector('#langForm input[name="__RequestVerificationToken"]');
    if (token) campo('__RequestVerificationToken', token.value);

    document.body.appendChild(form);
    form.submit();
  };

  // ── Afiche en grande (cartelera) ──────────────────────────────
  // El texto del afiche va dentro de la imagen y en el muro queda chico.
  // Sin soporte de <dialog> se deja pasar el enlace y abre la imagen.
  var grande = document.getElementById('afiche-grande');

  if (grande && typeof grande.showModal === 'function') {
    var imgGrande = grande.querySelector('img');

    document.querySelectorAll('.muro a.afiche').forEach(function (enlace) {
      enlace.addEventListener('click', function (e) {
        var img = enlace.querySelector('img');
        if (!img) return;
        e.preventDefault();
        imgGrande.src = enlace.href;
        imgGrande.alt = img.alt;
        grande.showModal();
      });
    });

    // Cierra con el botón o tocando fuera de la imagen (el fondo es el propio dialog).
    grande.addEventListener('click', function (e) {
      if (e.target === grande || e.target.closest('[data-cerrar]')) grande.close();
    });

    // Al cerrar se vacía, para que el próximo no muestre un instante el anterior.
    grande.addEventListener('close', function () {
      imgGrande.removeAttribute('src');
      imgGrande.alt = '';
    });
  }

  // ── Filtros por barrio (archivo y cartelera) ──────────────────
  // Sin recargar cuando el filtrado es sólo visual; si el grupo lleva
  // data-servidor, se deja pasar el enlace y filtra el controlador.
  document.querySelectorAll('.filtros[data-cliente]').forEach(function (grupo) {
    var destino = document.querySelector(grupo.dataset.cliente);
    if (!destino) return;

    grupo.addEventListener('click', function (e) {
      var boton = e.target.closest('.filtro');
      if (!boton) return;

      grupo.querySelectorAll('.filtro').forEach(function (b) {
        b.setAttribute('aria-pressed', 'false');
      });
      boton.setAttribute('aria-pressed', 'true');

      var barrio = boton.dataset.barrio || '';
      destino.querySelectorAll('[data-barrio]').forEach(function (item) {
        item.hidden = barrio !== '' && item.dataset.barrio !== barrio;
      });
    });
  });
})();
