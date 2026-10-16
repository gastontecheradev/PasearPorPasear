/* ══════════════════════════════════════════════════════════════
   PASEAR POR PASEAR — comportamiento del panel
   Todo es mejora progresiva: sin JavaScript los formularios se
   envían igual, sólo se ven los tres idiomas a la vez.
   ══════════════════════════════════════════════════════════════ */
(function () {
  'use strict';

  // ── Menú en pantallas chicas ──────────────────────────────────
  var menuBoton = document.getElementById('admin-menu-boton');
  var nav = document.getElementById('admin-nav');

  if (menuBoton && nav) {
    var cerrarMenu = function () {
      nav.classList.remove('abierta');
      menuBoton.setAttribute('aria-expanded', 'false');
      menuBoton.textContent = 'Menú';
    };

    menuBoton.addEventListener('click', function () {
      var abierto = nav.classList.toggle('abierta');
      menuBoton.setAttribute('aria-expanded', abierto ? 'true' : 'false');
      menuBoton.textContent = abierto ? 'Cerrar' : 'Menú';
    });

    document.addEventListener('keydown', function (e) {
      if (e.key !== 'Escape' || !nav.classList.contains('abierta')) return;
      cerrarMenu();
      menuBoton.focus();
    });
  }

  // ── Idioma de los textos ──────────────────────────────────────
  // Un selector por pantalla cambia todos los campos a la vez.
  var selector = document.querySelector('[data-selector-idioma]');
  var NOTAS = {
    es: 'El español es obligatorio. Lo que quede sin traducir se muestra en español.',
    en: 'Traduciendo al inglés. Debajo de cada campo tenés el texto en español como referencia.',
    pt: 'Traduciendo al portugués. Debajo de cada campo tenés el texto en español como referencia.'
  };

  function valorDe(campo, idioma) {
    var control = campo.querySelector('.idioma-panel[data-idioma="' + idioma + '"] input, .idioma-panel[data-idioma="' + idioma + '"] textarea');
    return control ? control.value.trim() : '';
  }

  function contarFaltantes() {
    if (!selector) return;
    var campos = document.querySelectorAll('[data-campo]');
    ['en', 'pt'].forEach(function (idioma) {
      var faltan = 0;
      campos.forEach(function (campo) {
        if (valorDe(campo, 'es') !== '' && valorDe(campo, idioma) === '') faltan++;
      });
      var marca = selector.querySelector('[data-faltan="' + idioma + '"]');
      if (!marca) return;
      marca.textContent = faltan === 1 ? 'falta 1' : 'faltan ' + faltan;
      marca.hidden = faltan === 0;
    });
  }

  // El original en español se copia de lo que esté escrito en ese momento,
  // no de lo que había al cargar la página.
  function actualizarOriginales() {
    document.querySelectorAll('[data-campo]').forEach(function (campo) {
      var es = valorDe(campo, 'es');
      campo.querySelectorAll('[data-original]').forEach(function (bloque) {
        var texto = bloque.querySelector('[data-original-texto]');
        if (texto) texto.textContent = es;
        bloque.hidden = es === '';
      });
      // Los casilleros cortos usan el placeholder como referencia.
      ['en', 'pt'].forEach(function (idioma) {
        var control = campo.querySelector('.idioma-panel[data-idioma="' + idioma + '"] input');
        if (control && !campo.querySelector('[data-original]')) control.placeholder = es;
      });
    });
  }

  function mostrarIdioma(idioma) {
    document.querySelectorAll('.idioma-panel').forEach(function (panel) {
      panel.hidden = panel.dataset.idioma !== idioma;
    });
    if (selector) {
      selector.querySelectorAll('button[data-idioma]').forEach(function (b) {
        b.setAttribute('aria-pressed', b.dataset.idioma === idioma ? 'true' : 'false');
      });
      var nota = selector.querySelector('[data-nota-idioma]');
      if (nota) nota.textContent = NOTAS[idioma];
    }
    if (idioma !== 'es') actualizarOriginales();
  }

  if (selector) {
    selector.addEventListener('click', function (e) {
      var boton = e.target.closest('button[data-idioma]');
      if (boton) mostrarIdioma(boton.dataset.idioma);
    });
    document.addEventListener('input', function (e) {
      if (e.target.closest('[data-campo]')) contarFaltantes();
    });
    contarFaltantes();
  }

  // Si el navegador frena el envío por un campo oculto en otro idioma,
  // se cambia a ese idioma para que se vea qué falta.
  document.addEventListener('invalid', function (e) {
    var panel = e.target.closest('.idioma-panel');
    if (panel && panel.hidden) mostrarIdioma(panel.dataset.idioma);
  }, true);

  // ── Listados: filtros y buscador ──────────────────────────────
  var listado = document.querySelector('[data-listado]');

  if (listado) {
    var filas = Array.prototype.slice.call(listado.querySelectorAll('[data-fila]'));
    var buscador = document.querySelector('[data-buscador]');
    var botonesFiltro = Array.prototype.slice.call(document.querySelectorAll('[data-filtro-boton]'));
    var sinResultados = document.querySelector('[data-sin-resultados]');
    var filtro = '';

    // Sin tildes ni mayúsculas: «rosalia» encuentra «Rosalía».
    var normalizar = function (s) {
      return s.normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase();
    };
    filas.forEach(function (f) { f.dataset.buscable = normalizar(f.textContent); });

    var aplicar = function () {
      var texto = buscador ? normalizar(buscador.value.trim()) : '';
      var visibles = 0;
      filas.forEach(function (f) {
        var ok = (filtro === '' || f.dataset.filtro === filtro) &&
                 (texto === '' || f.dataset.buscable.indexOf(texto) !== -1);
        f.hidden = !ok;
        if (ok) visibles++;
      });
      if (sinResultados) sinResultados.hidden = visibles !== 0;
    };

    var elegirFiltro = function (clave) {
      filtro = clave;
      botonesFiltro.forEach(function (b) {
        b.setAttribute('aria-pressed', b.dataset.filtroBoton === clave ? 'true' : 'false');
      });
      aplicar();
    };

    botonesFiltro.forEach(function (b) {
      b.addEventListener('click', function () { elegirFiltro(b.dataset.filtroBoton); });
    });
    if (buscador) buscador.addEventListener('input', aplicar);

    var limpiar = document.querySelector('[data-limpiar]');
    if (limpiar) {
      limpiar.addEventListener('click', function () {
        if (buscador) buscador.value = '';
        elegirFiltro('');
      });
    }
  }

  // ── Formularios: cambios sin guardar ──────────────────────────
  var hayCambios = false;
  var enviando = false;

  document.querySelectorAll('form[data-editor]').forEach(function (form) {
    var barra = form.querySelector('[data-barra-guardar]');
    var estado = form.querySelector('[data-estado-guardado]');

    var marcar = function () {
      if (form.dataset.cambios === 'si') return;
      form.dataset.cambios = 'si';
      hayCambios = true;
      if (barra) barra.classList.add('con-cambios');
      if (estado) estado.textContent = 'Hay cambios sin guardar.';
    };

    form.addEventListener('input', marcar);
    form.addEventListener('change', marcar);

    form.addEventListener('submit', function (e) {
      var boton = e.submitter;

      if (boton && boton.dataset.confirmar && !window.confirm(boton.dataset.confirmar)) {
        e.preventDefault();
        return;
      }

      enviando = true;
      if (boton && boton.dataset.guardando) {
        // Se deshabilita después de que el navegador arma el envío.
        setTimeout(function () {
          boton.disabled = true;
          boton.textContent = boton.dataset.guardando;
        }, 0);
      }
    });
  });

  window.addEventListener('beforeunload', function (e) {
    if (!hayCambios || enviando) return;
    e.preventDefault();
    e.returnValue = '';
  });

  // Formularios sueltos que piden confirmación (fuera de los editores)
  document.querySelectorAll('form[data-confirmar]').forEach(function (f) {
    f.addEventListener('submit', function (e) {
      if (!window.confirm(f.dataset.confirmar)) e.preventDefault();
    });
  });

  // ── Vista previa de la imagen elegida ─────────────────────────
  var LIMITE = 6 * 1024 * 1024;

  document.querySelectorAll('[data-imagen-archivo]').forEach(function (input) {
    var contenedor = input.closest('[data-imagen]');
    if (!contenedor) return;
    var vista = contenedor.querySelector('[data-imagen-vista]');
    var error = contenedor.querySelector('[data-imagen-error]');
    var original = vista ? Array.prototype.slice.call(vista.childNodes) : [];
    var urlPrevia = null;

    var restaurar = function () {
      if (!vista) return;
      vista.replaceChildren.apply(vista, original);
      vista.classList.remove('es-nueva');
    };

    input.addEventListener('change', function () {
      var archivo = input.files && input.files[0];
      if (error) error.hidden = true;
      if (urlPrevia) { URL.revokeObjectURL(urlPrevia); urlPrevia = null; }

      if (!archivo) { restaurar(); return; }

      if (archivo.size > LIMITE) {
        var mb = (archivo.size / 1024 / 1024).toFixed(1).replace('.', ',');
        if (error) {
          error.textContent = 'Esta imagen pesa ' + mb + ' MB y el límite es 6 MB. Achicala y elegila de nuevo.';
          error.hidden = false;
        }
        input.value = '';
        restaurar();
        return;
      }

      if (!vista) return;
      urlPrevia = URL.createObjectURL(archivo);
      var img = document.createElement('img');
      img.src = urlPrevia;
      img.alt = '';
      var nota = document.createElement('span');
      nota.className = 'origen';
      nota.textContent = 'Imagen nueva: se sube al guardar.';
      vista.replaceChildren(img, nota);
      vista.classList.add('es-nueva');
    });
  });
})();
