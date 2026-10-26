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
        // Los enlaces se muestran sólo con su texto, sin la marca [texto](dirección).
        if (texto) texto.textContent = es.replace(/\[([^\[\]\r\n]+)\]\(([^()\s]+)\)/g, '$1');
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

  // ── Editor de textos ──────────────────────────────────────────
  // Cambia a la vista los textarea con data-editor por un editor donde se
  // escribe como en cualquier documento: Enter baja de renglón y los enlaces
  // se ponen sobre el texto seleccionado, con una barra que aparece encima.
  // El textarea queda oculto y al día: es lo que se envía. Guarda texto
  // simple, un renglón por línea y los enlaces como [texto](dirección).
  var MARCA = /\[([^\[\]\r\n]+)\]\(([^()\s]+)\)/g;
  var DIRECCION = /^(https:\/\/[^\s()]+|\/(?![\/\\])[^\s()\\]*)$/i;
  var areasEditor = document.querySelectorAll('textarea[data-editor]');

  if (areasEditor.length) {
    var esMac = /Mac|iPhone|iPad/.test(navigator.platform || navigator.userAgent);
    var tactil = window.matchMedia && window.matchMedia('(pointer: coarse)').matches;
    var resaltado = window.CSS && CSS.highlights && typeof Highlight === 'function';
    var editores = [];

    var paginas = [];
    try {
      var datosPaginas = document.getElementById('paginas-del-sitio');
      if (datosPaginas) paginas = JSON.parse(datosPaginas.textContent);
    } catch (err) { paginas = []; }

    var ICONO_ENLACE = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" ' +
      'stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false">' +
      '<path d="M10 14a4.5 4.5 0 0 0 6.4 0l3-3a4.5 4.5 0 0 0-6.4-6.4l-1.1 1.1"/>' +
      '<path d="M14 10a4.5 4.5 0 0 0-6.4 0l-3 3a4.5 4.5 0 0 0 6.4 6.4l1.1-1.1"/></svg>';

    var sinTildes = function (s) {
      return s.normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase();
    };

    // Lo que se pega como dirección, llevado a lo que acepta el sitio:
    // «instagram.com/x» y «www.…» pasan a https://, y http:// a https://.
    var normalizarDireccion = function (d) {
      d = d.trim();
      if (!d) return '';
      if (/^http:\/\//i.test(d)) d = 'https://' + d.slice(7);
      else if (!/^https:\/\//i.test(d) && d.charAt(0) !== '/' && /^[^\s\/@]+\.[a-z]{2,}([\/?#]|$)/i.test(d)) d = 'https://' + d;
      return d.replace(/\(/g, '%28').replace(/\)/g, '%29');
    };

    // ── Texto guardado ⇄ lo que se ve en el editor ──
    // Un párrafo por renglón; los renglones en blanco del medio quedan como
    // párrafos vacíos, igual que en el sitio.
    var pintar = function (editor, valor) {
      var renglones = valor.split(/\r?\n/).map(function (r) { return r.trim() === '' ? '' : r; });
      while (renglones.length && renglones[0] === '') renglones.shift();
      while (renglones.length && renglones[renglones.length - 1] === '') renglones.pop();
      if (renglones.length === 0) renglones = [''];
      editor.replaceChildren.apply(editor, renglones.map(function (r) {
        var p = document.createElement('p');
        var desde = 0;
        var m;
        MARCA.lastIndex = 0;
        while ((m = MARCA.exec(r)) !== null) {
          if (!DIRECCION.test(m[2])) continue;
          if (m.index > desde) p.appendChild(document.createTextNode(r.slice(desde, m.index)));
          var a = document.createElement('a');
          a.setAttribute('href', m[2]);
          a.textContent = m[1];
          p.appendChild(a);
          desde = MARCA.lastIndex;
        }
        if (desde < r.length) p.appendChild(document.createTextNode(r.slice(desde)));
        if (!p.firstChild) p.appendChild(document.createElement('br'));
        return p;
      }));
    };

    // Lo que se ve, renglón por renglón: cada párrafo es un renglón y cada
    // salto (Shift+Enter) también. Los renglones en blanco cuentan, salvo al
    // principio y al final. Cualquier otro formato que se cuele (al deshacer,
    // al pegar) se queda sólo con el texto.
    var BLOQUE = /^(P|DIV|LI|H[1-6]|BLOCKQUOTE|UL|OL)$/;
    var serializar = function (editor) {
      var renglones = [];
      var bloque = null;
      var abrir = function () { if (!bloque) bloque = ['']; };
      var agregar = function (s) { abrir(); bloque[bloque.length - 1] += s; };
      var cerrar = function () {
        if (!bloque) return;
        // El <br> del final de un párrafo no se ve como renglón: es el que pone
        // el navegador para que el párrafo tenga altura.
        if (bloque.length > 1 && bloque[bloque.length - 1] === '') bloque.pop();
        renglones.push.apply(renglones, bloque);
        bloque = null;
      };
      var recorrer = function (nodo) {
        Array.prototype.forEach.call(nodo.childNodes, function (n) {
          if (n.nodeType === 3) {
            // Espacios con saltos de línea entre etiquetas: formato del HTML, no texto.
            if (/^\s*$/.test(n.nodeValue) && /[\r\n]/.test(n.nodeValue)) return;
            agregar(n.nodeValue.replace(/[\r\n]+/g, ' '));
            return;
          }
          if (n.nodeType !== 1) return;
          if (n.tagName === 'BR') { abrir(); bloque.push(''); return; }
          if (n.tagName === 'A') {
            var crudo = n.textContent.replace(/[\[\]\r\n]/g, ' ');
            var t = crudo.replace(/\s+/g, ' ').trim();
            var d = n.getAttribute('href') || '';
            if (!t || !DIRECCION.test(d)) { agregar(crudo); return; }
            // Los espacios de los bordes quedan afuera de la marca.
            agregar((/^\s/.test(crudo) ? ' ' : '') + '[' + t + '](' + d + ')' + (/\s$/.test(crudo) ? ' ' : ''));
            return;
          }
          if (BLOQUE.test(n.tagName)) {
            cerrar();
            // Un párrafo vacío también es un renglón; una lista sólo vale por sus ítems.
            if (n.tagName !== 'UL' && n.tagName !== 'OL') abrir();
            recorrer(n);
            cerrar();
            return;
          }
          recorrer(n);
        });
      };
      recorrer(editor);
      cerrar();
      var limpios = renglones.map(function (r) { return r.replace(/\s+/g, ' ').trim(); });
      while (limpios.length && limpios[0] === '') limpios.shift();
      while (limpios.length && limpios[limpios.length - 1] === '') limpios.pop();
      return limpios.join('\n');
    };

    var editorDe = function (nodo) {
      var el = nodo && (nodo.nodeType === 1 ? nodo : nodo.parentElement);
      var caja = el && el.closest('.editor-texto');
      if (!caja) return null;
      for (var i = 0; i < editores.length; i++) if (editores[i].editor === caja) return editores[i];
      return null;
    };

    var enlaceDe = function (nodo, editor) {
      var el = nodo && (nodo.nodeType === 1 ? nodo : nodo.parentElement);
      var a = el && el.closest('a');
      return a && editor.contains(a) ? a : null;
    };

    var nombreDestino = function (d) {
      for (var i = 0; i < paginas.length; i++) if (paginas[i].dir === d) return paginas[i].nombre;
      var corto = d.replace(/^https:\/\//i, '').replace(/^www\./i, '').replace(/\/$/, '');
      return corto.length > 38 ? corto.slice(0, 37) + '…' : corto;
    };

    var crear = function (etiqueta, atributos, hijos) {
      var el = document.createElement(etiqueta);
      Object.keys(atributos || {}).forEach(function (k) {
        if (atributos[k] === true) el.setAttribute(k, '');
        else if (atributos[k] !== false && atributos[k] != null) el.setAttribute(k, atributos[k]);
      });
      (hijos || []).forEach(function (h) { el.append(h); });
      return el;
    };

    var boton = function (texto, accion, clase, icono) {
      var b = crear('button', { type: 'button', class: clase || null }, [texto]);
      if (icono) b.insertAdjacentHTML('afterbegin', icono);
      b.addEventListener('click', accion);
      return b;
    };

    // ── La barra flotante ──
    // Un solo elemento para toda la pantalla. Estados: «barra» (hay texto
    // seleccionado), «enlace» (el cursor está en un enlace), «formulario»
    // (se está eligiendo la dirección) y «aviso».
    var flotante = crear('div', { class: 'editor-flotante', hidden: true });
    document.body.appendChild(flotante);
    var estado = null;
    var activo = null;
    var rango = null;
    var enlaceActual = null;
    var ancla = null;
    var temporizador = null;
    // La última selección que mostró la barra: en pantallas táctiles, tocar
    // un botón puede borrar la selección antes de que llegue el clic.
    var ultimoRango = null;
    // Mientras se escribe dentro de un enlace, la barra no tapa el texto.
    var porTeclado = false;

    var ocultar = function () {
      flotante.hidden = true;
      flotante.className = 'editor-flotante';
      flotante.replaceChildren();
      estado = null;
      rango = null;
      enlaceActual = null;
      ancla = null;
      clearTimeout(temporizador);
      if (resaltado) CSS.highlights.delete('editor-enlace');
    };

    var ubicar = function () {
      if (!ancla) return;
      var rect = ancla();
      flotante.hidden = false;
      var ancho = flotante.offsetWidth;
      var alto = flotante.offsetHeight;
      var x = rect.left + rect.width / 2 - ancho / 2;
      x = Math.max(8, Math.min(x, document.documentElement.clientWidth - ancho - 8));
      var arriba = rect.top - alto - 10;
      // En pantallas táctiles el menú del sistema ocupa el lugar de arriba.
      var y = tactil || arriba < 8 ? rect.bottom + 10 : arriba;
      flotante.style.left = (x + window.scrollX) + 'px';
      flotante.style.top = (y + window.scrollY) + 'px';
    };

    var rectDe = function (r) {
      var rect = r.getBoundingClientRect();
      return rect.width || rect.height ? rect : activo.editor.getBoundingClientRect();
    };

    var seleccionar = function (r) {
      var sel = window.getSelection();
      activo.editor.focus();
      sel.removeAllRanges();
      sel.addRange(r);
    };

    var mostrarBarra = function (r) {
      estado = 'barra';
      flotante.className = 'editor-flotante';
      flotante.replaceChildren(boton('Enlace', abrirFormulario, null, ICONO_ENLACE));
      var copia = r.cloneRange();
      ultimoRango = copia;
      ancla = function () { return rectDe(copia); };
      ubicar();
    };

    var mostrarEnlace = function (a) {
      estado = 'enlace';
      enlaceActual = a;
      flotante.className = 'editor-flotante';
      var d = a.getAttribute('href');
      var ir = crear('a', { class: 'editor-flotante-destino', href: d, target: '_blank', rel: 'noopener', title: d },
        [nombreDestino(d), crear('span', { class: 'oculto-visual' }, [' (se abre en otra pestaña)'])]);
      flotante.replaceChildren(
        crear('span', { class: 'editor-flotante-etiqueta' }, ['Lleva a']), ir,
        crear('span', { class: 'editor-flotante-separador', 'aria-hidden': 'true' }),
        boton('Cambiar', abrirFormulario),
        boton('Quitar', quitarEnlace));
      ancla = function () { return a.getBoundingClientRect(); };
      ubicar();
    };

    var mostrarAviso = function (r) {
      estado = 'aviso';
      flotante.className = 'editor-flotante';
      flotante.replaceChildren(crear('p', { class: 'editor-flotante-aviso', role: 'status' },
        ['Primero seleccioná el texto que va a ser el enlace.']));
      var copia = r.cloneRange();
      ancla = function () { return rectDe(copia); };
      ubicar();
      temporizador = setTimeout(ocultar, 2600);
    };

    var quitarEnlace = function () {
      var a = enlaceActual;
      if (!a || !activo) return;
      var r = document.createRange();
      r.selectNodeContents(a);
      seleccionar(r);
      document.execCommand('unlink');
      window.getSelection().collapseToEnd();
      ocultar();
    };

    function abrirFormulario() {
      var sel = window.getSelection();
      if (!activo) return;
      var r = sel.rangeCount && activo.editor.contains(sel.getRangeAt(0).commonAncestorContainer)
        ? sel.getRangeAt(0)
        : ultimoRango && activo.editor.contains(ultimoRango.commonAncestorContainer) ? ultimoRango : null;
      if (!r) return;
      var a = enlaceDe(r.startContainer, activo.editor) || enlaceDe(r.endContainer, activo.editor);
      if (a) {
        r = document.createRange();
        r.selectNodeContents(a);
      } else if (r.collapsed) {
        mostrarAviso(r);
        return;
      }
      rango = r.cloneRange();
      enlaceActual = a;
      estado = 'formulario';
      var editorActivo = activo;
      if (resaltado) CSS.highlights.set('editor-enlace', new Highlight(rango));

      var campo = crear('input', {
        type: 'text', inputmode: 'url', autocomplete: 'off', spellcheck: 'false',
        class: 'editor-flotante-campo', role: 'combobox', 'aria-expanded': 'false',
        'aria-autocomplete': 'list', 'aria-controls': 'editor-paginas',
        'aria-label': 'A dónde lleva el enlace', placeholder: 'Pegá una dirección o buscá una página'
      });
      var titulo = crear('p', { class: 'editor-flotante-titulo', id: 'editor-paginas-titulo' }, ['Páginas del sitio']);
      var lista = crear('ul', { class: 'editor-flotante-paginas', id: 'editor-paginas', role: 'listbox', 'aria-labelledby': 'editor-paginas-titulo' });
      var error = crear('p', { class: 'editor-flotante-error', role: 'alert', hidden: true });
      var opciones = [];
      var indice = -1;

      var marcarOpcion = function (i) {
        indice = i;
        Array.prototype.forEach.call(lista.children, function (li, j) {
          li.setAttribute('aria-selected', j === i ? 'true' : 'false');
        });
        if (i >= 0) {
          campo.setAttribute('aria-activedescendant', 'editor-pagina-' + i);
          lista.children[i].scrollIntoView({ block: 'nearest' });
        } else {
          campo.removeAttribute('aria-activedescendant');
        }
      };

      // Si lo escrito parece una dirección, la lista se aparta; si parece
      // el nombre de una página, la primera que coincide queda elegida.
      var filtrar = function () {
        var escrito = campo.value.trim();
        var q = sinTildes(escrito).replace(/^\//, '');
        var pareceDireccion = /^https?:|\.[a-z]{2,}/i.test(escrito);
        opciones = pareceDireccion ? [] : paginas.filter(function (p) {
          return !q || sinTildes(p.nombre).indexOf(q) !== -1 || sinTildes(p.dir).slice(1).indexOf(q) === 0;
        });
        lista.replaceChildren.apply(lista, opciones.map(function (p, i) {
          var li = crear('li', { id: 'editor-pagina-' + i, role: 'option', 'aria-selected': 'false' },
            [crear('b', {}, [p.nombre]), crear('span', {}, [p.dir])]);
          li.addEventListener('click', function () { aplicar(p.dir); });
          return li;
        }));
        titulo.hidden = lista.hidden = opciones.length === 0;
        campo.setAttribute('aria-expanded', opciones.length ? 'true' : 'false');
        marcarOpcion(q && opciones.length ? 0 : -1);
      };

      var aplicar = function (d) {
        if (!d) {
          error.textContent = 'Pegá una dirección o elegí una página de la lista.';
          error.hidden = false;
          campo.focus();
          return;
        }
        if (!DIRECCION.test(d)) {
          error.textContent = 'Eso no parece una dirección. Pegala completa, por ejemplo https://instagram.com/pasearporpasear, o elegí una página del sitio.';
          error.hidden = false;
          campo.focus();
          return;
        }
        var r2 = rango;
        ocultar();
        activo = editorActivo;
        seleccionar(r2);
        document.execCommand('createLink', false, d);
      };

      var cancelar = function () {
        var r2 = rango;
        ocultar();
        activo = editorActivo;
        seleccionar(r2);
      };

      campo.addEventListener('input', function () { error.hidden = true; filtrar(); });
      campo.addEventListener('keydown', function (e) {
        if (e.key === 'ArrowDown' && opciones.length) {
          e.preventDefault();
          marcarOpcion((indice + 1) % opciones.length);
        } else if (e.key === 'ArrowUp' && opciones.length) {
          e.preventDefault();
          marcarOpcion(indice <= 0 ? opciones.length - 1 : indice - 1);
        } else if (e.key === 'Enter') {
          e.preventDefault();
          aplicar(indice >= 0 ? opciones[indice].dir : normalizarDireccion(campo.value));
        } else if (e.key === 'Escape') {
          e.preventDefault();
          cancelar();
        }
      });

      var acciones = [boton(a ? 'Guardar el enlace' : 'Poner el enlace', function () {
        aplicar(indice >= 0 ? opciones[indice].dir : normalizarDireccion(campo.value));
      }, 'editor-flotante-aplicar')];
      if (a) acciones.push(boton('Quitar el enlace', quitarEnlace));
      acciones.push(boton('Cancelar', cancelar));

      flotante.className = 'editor-flotante editor-flotante--formulario';
      flotante.replaceChildren(campo, titulo, lista, error, crear('div', { class: 'editor-flotante-acciones' }, acciones));
      if (a) campo.value = a.getAttribute('href');
      filtrar();
      if (a) marcarOpcion(-1);
      var copia = rango;
      ancla = function () { return rectDe(copia); };
      ubicar();
      campo.focus();
      campo.select();
    }

    // Qué mostrar según dónde quedó la selección.
    var revisarSeleccion = function () {
      if (estado === 'formulario') return;
      var sel = window.getSelection();
      if (!sel.rangeCount) { ocultar(); return; }
      var r = sel.getRangeAt(0);
      var ed = editorDe(r.commonAncestorContainer);
      if (!ed || document.activeElement !== ed.editor) { ocultar(); return; }
      activo = ed;
      var a = enlaceDe(r.startContainer, ed.editor);
      if (a && (r.collapsed || enlaceDe(r.endContainer, ed.editor) === a)) {
        if (r.collapsed && porTeclado) { ocultar(); return; }
        if (estado !== 'enlace' || enlaceActual !== a) mostrarEnlace(a);
        else ubicar();
        return;
      }
      if (!r.collapsed) { mostrarBarra(r); return; }
      if (estado !== 'aviso') ocultar();
    };

    var pendiente = 0;
    document.addEventListener('selectionchange', function () {
      cancelAnimationFrame(pendiente);
      pendiente = requestAnimationFrame(revisarSeleccion);
    });

    // Un clic afuera cierra la barra; los botones de la barra no le sacan
    // el foco al editor, para no perder la selección.
    document.addEventListener('mousedown', function (e) {
      if (flotante.hidden || flotante.contains(e.target)) return;
      if (estado === 'formulario' || !(activo && activo.editor.contains(e.target))) ocultar();
    }, true);
    flotante.addEventListener('mousedown', function (e) {
      if (!e.target.closest('input')) e.preventDefault();
    });
    // Escribir en la dirección no es un cambio del formulario de la página.
    ['input', 'change'].forEach(function (tipo) {
      flotante.addEventListener(tipo, function (e) { e.stopPropagation(); });
    });
    window.addEventListener('scroll', function () { if (!flotante.hidden) ubicar(); }, { passive: true });
    window.addEventListener('resize', function () { if (!flotante.hidden) ubicar(); });

    // Enter crea un párrafo (<p>) en todos los navegadores, no un <div>.
    try { document.execCommand('defaultParagraphSeparator', false, 'p'); } catch (err) { /* sin soporte: se serializa igual */ }

    areasEditor.forEach(function (area) {
      var panel = area.closest('.idioma-panel') || area.parentNode;
      var modo = area.dataset.editor;
      var etiqueta = panel.querySelector('label[for="' + area.id + '"]');
      var editor = crear('div', {
        class: 'editor-texto ' + area.className, contenteditable: 'true', role: 'textbox',
        'aria-multiline': 'true', spellcheck: 'true', lang: panel.dataset.idioma || 'es', 'data-modo': modo
      });
      var ed = { editor: editor, area: area };
      editores.push(ed);

      if (etiqueta) {
        editor.setAttribute('aria-labelledby', etiqueta.id);
        etiqueta.addEventListener('click', function (e) { e.preventDefault(); editor.focus(); });
      }

      pintar(editor, area.value);
      area.hidden = true;
      area.parentNode.insertBefore(editor, area);

      var ayuda = panel.querySelector('[data-editor-ayuda]');
      if (ayuda) {
        ayuda.id = area.id + '-ayuda';
        ayuda.textContent = (modo === 'parrafos' ? 'Enter empieza un párrafo nuevo.' : 'Enter baja al renglón siguiente.') +
          ' Para poner un enlace, seleccioná el texto y tocá Enlace' + (tactil ? '.' : ' (o ' + (esMac ? '⌘K' : 'Ctrl+K') + ').');
        editor.setAttribute('aria-describedby', ayuda.id);
      }

      // El límite de caracteres del textarea, contado sobre lo que se guarda.
      var maximo = area.maxLength > 0 ? area.maxLength : 0;
      var contador = null;
      if (maximo) {
        contador = crear('p', { class: 'editor-contador', id: area.id + '-contador' });
        area.parentNode.insertBefore(contador, area.nextSibling);
        editor.setAttribute('aria-describedby', ((editor.getAttribute('aria-describedby') || '') + ' ' + contador.id).trim());
      }
      var contar = function () {
        if (!contador) return;
        // Al enviarse, cada salto de línea viaja como dos caracteres (\r\n):
        // se cuenta igual que lo cuenta el servidor.
        var largo = area.value.replace(/\n/g, '\r\n').length;
        var sobra = largo - maximo;
        contador.classList.toggle('excedido', sobra > 0);
        contador.textContent = sobra > 0
          ? 'Te pasaste por ' + sobra + (sobra === 1 ? ' carácter' : ' caracteres') + ': el máximo es ' + maximo + '.'
          : largo + ' de ' + maximo;
      };
      contar();

      editor.addEventListener('input', function () {
        area.value = serializar(editor);
        contar();
      });

      editor.addEventListener('keydown', function (e) {
        var mod = (e.ctrlKey || e.metaKey) && !e.altKey;
        var tecla = e.key.toLowerCase();
        // Con las flechas se puede llegar a un enlace y ver a dónde lleva;
        // escribiendo, no.
        porTeclado = !mod && (e.key.length === 1 || /^(Backspace|Delete|Enter)$/.test(e.key));
        if (mod && tecla === 'k') {
          e.preventDefault();
          activo = ed;
          abrirFormulario();
        } else if (mod && (tecla === 'b' || tecla === 'i' || tecla === 'u')) {
          // Negrita y cursiva no se guardan: mejor que no aparezcan.
          e.preventDefault();
        } else if (e.key === 'Escape' && !flotante.hidden) {
          ocultar();
        }
      });

      // Se pega sólo el texto. Una dirección pegada sobre texto seleccionado
      // lo convierte en enlace, sin abrir nada.
      editor.addEventListener('paste', function (e) {
        var datos = e.clipboardData || window.clipboardData;
        if (!datos) return;
        e.preventDefault();
        var texto = (datos.getData('text/plain') || '').replace(/\r\n?/g, '\n');
        var sel = window.getSelection();
        var limpio = texto.trim();
        if (sel.rangeCount && !sel.isCollapsed && /^(https?:\/\/|www\.)\S+$/i.test(limpio) &&
            DIRECCION.test(normalizarDireccion(limpio))) {
          document.execCommand('createLink', false, normalizarDireccion(limpio));
          return;
        }
        texto.split('\n').forEach(function (renglon, i) {
          if (i > 0) document.execCommand('insertParagraph');
          if (renglon) document.execCommand('insertText', false, renglon);
        });
      });

      editor.addEventListener('pointerdown', function () { porTeclado = false; });

      // Lo que se arrastra de afuera puede traer formato: no se acepta.
      editor.addEventListener('drop', function (e) { e.preventDefault(); });

      editor.addEventListener('blur', function () {
        setTimeout(function () {
          if (estado !== 'formulario' && !flotante.contains(document.activeElement)) ocultar();
        }, 0);
      });
    });
  }

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
