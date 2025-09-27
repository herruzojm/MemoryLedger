const diaryList = document.getElementById("diary-list");
const messageBar = document.getElementById("message");
const diaryView = document.getElementById("diary-view");
const diaryTitle = document.getElementById("diary-title");
const diarySubtitle = document.getElementById("diary-subtitle");
const entriesContainer = document.getElementById("entries");
const entriesEmpty = document.getElementById("entries-empty");
const averageResult = document.getElementById("average-result");

const createForm = document.getElementById("create-diary-form");
const openForm = document.getElementById("open-diary-form");
const deleteDiaryButton = document.getElementById("delete-diary");
const refreshButton = document.getElementById("refresh-diary");
const closeButton = document.getElementById("close-diary");
const addEntryButton = document.getElementById("add-entry");
const searchForm = document.getElementById("search-form");
const resetSearchButton = document.getElementById("reset-search");
const averageForm = document.getElementById("average-form");
const entryModal = document.getElementById("entry-modal");
const entryModalTitle = document.getElementById("entry-modal-title");
const entryForm = document.getElementById("entry-form");
const entrySubmitButton = document.getElementById("entry-submit");
const entryCancelButton = document.getElementById("entry-cancel");
const entryCloseButton = document.getElementById("entry-close");

const entryDateInput = document.getElementById("entry-date");
const entryTitleInput = document.getElementById("entry-title");
const entryDescriptionInput = document.getElementById("entry-description");
const entryIntensityInput = document.getElementById("entry-intensity");

let currentDiary = null;
let currentPassword = null;
let entriesCache = [];
let editingEntryId = null;

const getToday = () => new Date().toISOString().slice(0, 10);
entryDateInput.value = getToday();

init();

function init() {
  loadDiaries();

  createForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const formData = new FormData(createForm);
    const payload = Object.fromEntries(formData.entries());

    const response = await apiPost("/api/diaries/create", payload);
    showMessage(response.message, response.ok ? "success" : "error");

    if (response.ok) {
      createForm.reset();
      loadDiaries();
    }
  });

  openForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    await openDiaryFromForm();
  });

  deleteDiaryButton.addEventListener("click", async () => {
    const name = openForm.name.value.trim();
    const password = openForm.password.value.trim();

    if (!name || !password) {
      showMessage("Provide both the diary name and password to delete it.", "error");
      return;
    }

    if (!confirm(`Delete the diary "${name}"? This action cannot be undone.`)) {
      return;
    }

    const response = await apiPost("/api/diaries/delete", { name, password });
    showMessage(response.message, response.ok ? "success" : "error");

    if (response.ok) {
      if (currentDiary === name) {
        closeDiaryView();
      }
      loadDiaries();
      openForm.reset();
    }
  });

  refreshButton.addEventListener("click", async () => {
    if (!currentDiary || !currentPassword) {
      showMessage("Open a diary first.", "error");
      return;
    }

    await openDiary(currentDiary, currentPassword, { silent: true });
  });

  closeButton.addEventListener("click", () => {
    closeDiaryView();
    showMessage("Diary closed.", "success");
  });

  addEntryButton.addEventListener("click", () => {
    if (!ensureDiaryOpen()) {
      return;
    }

    openEntryModal("create");
  });

  entryCancelButton.addEventListener("click", () => {
    closeEntryModal();
  });

  entryCloseButton.addEventListener("click", () => {
    closeEntryModal();
  });

  entryModal.addEventListener("click", (event) => {
    if (event.target === entryModal) {
      closeEntryModal();
    }
  });

  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape" && !entryModal.classList.contains("hidden")) {
      closeEntryModal();
    }
  });

  entryForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (!ensureDiaryOpen()) {
      closeEntryModal();
      return;
    }

    const formData = new FormData(entryForm);
    const payload = Object.fromEntries(formData.entries());
    payload.diaryName = currentDiary;
    payload.password = currentPassword;
    payload.intensity = Number(payload.intensity);

    let response;
    if (editingEntryId !== null) {
      payload.entryId = editingEntryId;
      response = await apiPost("/api/entries/update", payload);
    } else {
      response = await apiPost("/api/entries/add", payload);
    }

    showMessage(response.message, response.ok ? "success" : "error");

    if (response.ok) {
      closeEntryModal();
      await openDiary(currentDiary, currentPassword, { silent: true });
    }
  });

  searchForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (!ensureDiaryOpen()) {
      return;
    }

    const formData = new FormData(searchForm);
    const payload = Object.fromEntries(formData.entries());
    payload.diaryName = currentDiary;
    payload.password = currentPassword;
    if (!payload.text) delete payload.text;
    if (!payload.date) delete payload.date;
    if (!payload.intensity) delete payload.intensity;

    const response = await apiPost("/api/entries/search", payload);
    showMessage(response.message, response.ok ? "success" : "error");

    if (response.ok && response.data) {
      renderEntries(response.data, { fromSearch: true });
    }
  });

  resetSearchButton.addEventListener("click", () => {
    searchForm.reset();
    entryDateInput.value = getToday();
    averageResult.textContent = "";
    renderEntries(entriesCache);
  });

  averageForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (!ensureDiaryOpen()) {
      return;
    }

    const formData = new FormData(averageForm);
    const payload = Object.fromEntries(formData.entries());
    payload.diaryName = currentDiary;
    payload.password = currentPassword;
    if (!payload.start) delete payload.start;
    if (!payload.end) delete payload.end;

    const response = await apiPost("/api/entries/average", payload);
    showMessage(response.message, response.ok ? "success" : "error");

    if (response.ok && response.data) {
      const { average, start, end } = response.data;
      averageResult.textContent = average === null
        ? "No memories in the selected range."
        : `Average intensity from ${formatDate(start)} to ${formatDate(end)}: ${Number(average).toFixed(2)}`;
    }
  });

  entriesContainer.addEventListener("click", async (event) => {
    const button = event.target.closest("button[data-action]");
    if (!button) return;

    const entryId = Number(button.dataset.entryId);
    if (Number.isNaN(entryId)) return;

    if (button.dataset.action === "delete") {
      await deleteEntry(entryId);
    } else if (button.dataset.action === "edit") {
      editEntry(entryId);
    }
  });
}

async function openDiaryFromForm() {
  const name = openForm.name.value.trim();
  const password = openForm.password.value.trim();

  if (!name || !password) {
    showMessage("Both name and password are required to open a diary.", "error");
    return;
  }

  await openDiary(name, password);
}

async function openDiary(name, password, options = {}) {
  const response = await apiPost("/api/diaries/open", { name, password });
  showMessage(response.message, response.ok ? "success" : "error", options.silent);

  if (!response.ok || !response.data) {
    return;
  }

  currentDiary = response.data.name;
  currentPassword = password;
  entriesCache = response.data.entries ?? [];

  diaryTitle.textContent = currentDiary;
  diarySubtitle.textContent = `${entriesCache.length} ${entriesCache.length === 1 ? "memory" : "memories"}`;
  averageResult.textContent = "";

  renderEntries(entriesCache);
  diaryView.classList.remove("hidden");
}

function closeDiaryView() {
  currentDiary = null;
  currentPassword = null;
  entriesCache = [];
  diaryTitle.textContent = "";
  diarySubtitle.textContent = "";
  averageResult.textContent = "";
  entriesContainer.innerHTML = "";
  entriesEmpty.classList.remove("hidden");
  diaryView.classList.add("hidden");
}

function ensureDiaryOpen() {
  if (!currentDiary || !currentPassword) {
    showMessage("Open a diary first.", "error");
    return false;
  }
  return true;
}

async function deleteEntry(entryId) {
  if (!ensureDiaryOpen()) {
    return;
  }

  if (!confirm("Delete this memory?")) {
    return;
  }

  const payload = {
    diaryName: currentDiary,
    password: currentPassword,
    entryId,
  };

  const response = await apiPost("/api/entries/delete", payload);
  showMessage(response.message, response.ok ? "success" : "error");

  if (response.ok) {
    await openDiary(currentDiary, currentPassword, { silent: true });
  }
}

function editEntry(entryId) {
  if (!ensureDiaryOpen()) {
    return;
  }

  const entry = entriesCache.find((item) => item.id === entryId);
  if (!entry) {
    showMessage("Memory not found in the current view.", "error");
    return;
  }

  openEntryModal("edit", entry);
}

function openEntryModal(mode, entry) {
  editingEntryId = mode === "edit" && entry ? entry.id : null;

  entryForm.reset();

  if (mode === "edit" && entry) {
    entryModalTitle.textContent = "Edit memory";
    entrySubmitButton.textContent = "Update memory";
    entryDateInput.value = formatDate(entry.date);
    entryTitleInput.value = entry.title;
    entryDescriptionInput.value = entry.description;
    entryIntensityInput.value = entry.intensity;
  } else {
    entryModalTitle.textContent = "Add memory";
    entrySubmitButton.textContent = "Save memory";
    entryDateInput.value = getToday();
    entryTitleInput.value = "";
    entryDescriptionInput.value = "";
    entryIntensityInput.value = 5;
  }

  entryModal.classList.remove("hidden");
  document.body.classList.add("modal-open");
  entryTitleInput.focus();
}

function closeEntryModal() {
  editingEntryId = null;
  entryModal.classList.add("hidden");
  document.body.classList.remove("modal-open");
  entryForm.reset();
  entryDateInput.value = getToday();
  entryIntensityInput.value = 5;
}

function renderEntries(entries, options = {}) {
  entriesContainer.innerHTML = "";

  const list = Array.isArray(entries) ? entries : [];
  if (list.length === 0) {
    entriesEmpty.classList.remove("hidden");
    return;
  }

  if (!options.fromSearch) {
    entriesCache = list;
  }

  entriesEmpty.classList.add("hidden");

  list.forEach((entry) => {
    const element = document.createElement("article");
    element.className = "entry";

    const header = document.createElement("div");
    header.className = "entry-header";

    const title = document.createElement("div");
    title.className = "entry-title";
    title.textContent = entry.title;

    const actions = document.createElement("div");
    actions.className = "entry-actions";

    const editButton = document.createElement("button");
    editButton.type = "button";
    editButton.className = "secondary";
    editButton.dataset.action = "edit";
    editButton.dataset.entryId = entry.id;
    editButton.textContent = "Edit";

    const deleteButton = document.createElement("button");
    deleteButton.type = "button";
    deleteButton.className = "danger";
    deleteButton.dataset.action = "delete";
    deleteButton.dataset.entryId = entry.id;
    deleteButton.textContent = "Delete";

    actions.append(editButton, deleteButton);
    header.append(title, actions);

    const meta = document.createElement("div");
    meta.className = "entry-meta";
    meta.textContent = `${formatDate(entry.date)} • Intensity ${entry.intensity}`;

    const description = document.createElement("p");
    description.textContent = entry.description;

    element.append(header, meta, description);
    entriesContainer.append(element);
  });
}

function showMessage(text, type = "success", silent = false) {
  if (silent || !text) {
    return;
  }

  messageBar.textContent = text;
  messageBar.classList.remove("hidden", "success", "error");
  messageBar.classList.add(type === "error" ? "error" : "success");

  clearTimeout(showMessage.timeoutId);
  showMessage.timeoutId = setTimeout(() => {
    messageBar.classList.add("hidden");
  }, 5000);
}

async function loadDiaries() {
  const response = await apiGet("/api/diaries");
  if (!response.ok || !response.data) {
    showMessage(response.message || "Unable to load diaries.", "error");
    return;
  }

  diaryList.innerHTML = "";
  if (response.data.length === 0) {
    const empty = document.createElement("li");
    empty.textContent = "No diaries yet.";
    empty.className = "hint";
    diaryList.append(empty);
    return;
  }

  response.data.forEach((name) => {
    const item = document.createElement("li");
    item.textContent = name;
    item.tabIndex = 0;
    item.addEventListener("click", () => {
      openForm.name.value = name;
      openForm.name.focus();
    });
    item.addEventListener("keydown", (event) => {
      if (event.key === "Enter" || event.key === " ") {
        event.preventDefault();
        openForm.name.value = name;
        openForm.name.focus();
      }
    });
    diaryList.append(item);
  });
}

async function apiPost(url, payload) {
  try {
    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });

    const data = await safeJson(response);
    return { ok: response.ok, status: response.status, ...data };
  } catch (error) {
    console.error(error);
    return { ok: false, status: 0, message: "Network error." };
  }
}

async function apiGet(url) {
  try {
    const response = await fetch(url);
    const data = await safeJson(response);
    return { ok: response.ok, status: response.status, ...data };
  } catch (error) {
    console.error(error);
    return { ok: false, status: 0, message: "Network error." };
  }
}

async function safeJson(response) {
  try {
    const data = await response.json();
    return {
      message: data.message,
      data: data.data,
    };
  } catch {
    return { message: "Unexpected server response." };
  }
}

function formatDate(value) {
  if (!value) return "";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return value;
  }
  return date.toISOString().slice(0, 10);
}
