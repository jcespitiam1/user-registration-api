const API_BASE = "/api";

const paisSelect = document.getElementById("pais");
const departamentoSelect = document.getElementById("departamento");
const municipioSelect = document.getElementById("municipio");
const formRegistro = document.getElementById("form-registro");
const registroResultado = document.getElementById("registro-resultado");
const formConsulta = document.getElementById("form-consulta");
const consultaResultado = document.getElementById("consulta-resultado");

async function apiFetch(path, options) {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  const contentType = response.headers.get("content-type") ?? "";
  const body = contentType.includes("json") ? await response.json() : null;

  if (!response.ok) {
    const error = new Error(body?.title ?? `Error HTTP ${response.status}`);
    error.status = response.status;
    error.problem = body;
    throw error;
  }

  return body;
}

function fillSelect(select, items, valueKey, labelKey, placeholder) {
  select.innerHTML = "";
  const placeholderOption = document.createElement("option");
  placeholderOption.value = "";
  placeholderOption.textContent = placeholder;
  select.appendChild(placeholderOption);

  for (const item of items) {
    const option = document.createElement("option");
    option.value = item[valueKey];
    option.textContent = item[labelKey];
    select.appendChild(option);
  }
}

function resetSelect(select, placeholder) {
  select.innerHTML = "";
  const placeholderOption = document.createElement("option");
  placeholderOption.value = "";
  placeholderOption.textContent = placeholder;
  select.appendChild(placeholderOption);
  select.disabled = true;
}

async function cargarPaises() {
  try {
    const paises = await apiFetch("/paises");
    fillSelect(paisSelect, paises, "idPais", "nombre", "Selecciona un país...");
  } catch (error) {
    console.error("No se pudieron cargar los países", error);
  }
}

paisSelect.addEventListener("change", async () => {
  resetSelect(departamentoSelect, "Selecciona un departamento...");
  resetSelect(municipioSelect, "Selecciona un municipio...");

  if (!paisSelect.value) return;

  const departamentos = await apiFetch(`/paises/${paisSelect.value}/departamentos`);
  fillSelect(departamentoSelect, departamentos, "idDepartamento", "nombre", "Selecciona un departamento...");
  departamentoSelect.disabled = false;
});

departamentoSelect.addEventListener("change", async () => {
  resetSelect(municipioSelect, "Selecciona un municipio...");

  if (!departamentoSelect.value) return;

  const municipios = await apiFetch(`/departamentos/${departamentoSelect.value}/municipios`);
  fillSelect(municipioSelect, municipios, "idMunicipio", "nombre", "Selecciona un municipio...");
  municipioSelect.disabled = false;
});

function limpiarErrores(form) {
  form.querySelectorAll(".error").forEach((el) => (el.textContent = ""));
  form.querySelectorAll(".invalid").forEach((el) => el.classList.remove("invalid"));
}

function mostrarErroresDeCampo(form, errors) {
  const camposPorClave = {
    numerodocumento: "numero-documento",
    nombre: "nombre",
    telefono: "telefono",
    idpais: "pais",
    iddepartamento: "departamento",
    idmunicipio: "municipio",
    direccion: "direccion",
  };

  for (const [campo, mensajes] of Object.entries(errors ?? {})) {
    const span = form.querySelector(`[data-error-for="${campo}"]`);
    if (span) span.textContent = mensajes.join(" ");

    const inputId = camposPorClave[campo.toLowerCase()];
    const input = inputId ? document.getElementById(inputId) : null;
    if (input) input.classList.add("invalid");
  }
}

function mostrarResultadoUsuario(contenedor, usuario) {
  contenedor.className = "resultado ok";
  contenedor.hidden = false;
  contenedor.innerHTML = `
    <dl>
      <dt>Documento</dt><dd>${usuario.numeroDocumento}</dd>
      <dt>Nombre</dt><dd>${usuario.nombre}</dd>
      <dt>Teléfono</dt><dd>${usuario.telefono}</dd>
      <dt>Ubicación</dt><dd>${usuario.municipioNombre}, ${usuario.departamentoNombre}, ${usuario.paisNombre}</dd>
      <dt>Dirección</dt><dd>${usuario.direccion}</dd>
      <dt>Fecha de registro</dt><dd>${new Date(usuario.fechaRegistro).toLocaleString()}</dd>
    </dl>
  `;
}

function mostrarError(contenedor, mensaje) {
  contenedor.className = "resultado fail";
  contenedor.hidden = false;
  contenedor.textContent = mensaje;
}

formRegistro.addEventListener("submit", async (event) => {
  event.preventDefault();
  limpiarErrores(formRegistro);
  registroResultado.hidden = true;

  const payload = {
    numeroDocumento: document.getElementById("numero-documento").value.trim(),
    nombre: document.getElementById("nombre").value.trim(),
    telefono: document.getElementById("telefono").value.trim(),
    idPais: Number(paisSelect.value),
    idDepartamento: Number(departamentoSelect.value),
    idMunicipio: Number(municipioSelect.value),
    direccion: document.getElementById("direccion").value.trim(),
  };

  try {
    const usuario = await apiFetch("/usuarios", {
      method: "POST",
      body: JSON.stringify(payload),
    });
    mostrarResultadoUsuario(registroResultado, usuario);
    formRegistro.reset();
    resetSelect(departamentoSelect, "Selecciona un departamento...");
    resetSelect(municipioSelect, "Selecciona un municipio...");
  } catch (error) {
    if (error.status === 400 && error.problem?.errors) {
      mostrarErroresDeCampo(formRegistro, error.problem.errors);
      mostrarError(registroResultado, "Revisa los campos marcados en el formulario.");
    } else if (error.status === 409) {
      mostrarErroresDeCampo(formRegistro, { NumeroDocumento: [error.problem?.detail ?? error.message] });
      mostrarError(registroResultado, error.problem?.detail ?? error.message);
    } else {
      mostrarError(registroResultado, error.problem?.detail ?? error.message);
    }
  }
});

formConsulta.addEventListener("submit", async (event) => {
  event.preventDefault();
  consultaResultado.hidden = true;

  const id = document.getElementById("consulta-id").value;

  try {
    const usuario = await apiFetch(`/usuarios/${id}`);
    mostrarResultadoUsuario(consultaResultado, usuario);
  } catch (error) {
    mostrarError(consultaResultado, error.problem?.detail ?? error.message);
  }
});

cargarPaises();
