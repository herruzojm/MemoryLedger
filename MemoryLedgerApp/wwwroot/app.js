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
const addEntryForm = document.getElementById("add-entry-form");
const searchForm = document.getElementById("search-form");
const resetSearchButton = document.getElementById("reset-search");
const averageForm = document.getElementById("average-form");

const entryDateInput = document.getElementById("entry-date");

let currentDiary = null;
let currentPassword = null;
let entriesCache = [];

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

  addEntryForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (!ensureDiaryOpen()) {
      return;
    }

    const formData = new FormData(addEntryForm);
    const payload = Object.fromEntries(formData.entries());
    payload.diaryName = currentDiary;
    payload.password = currentPassword;
    payload.intensity = Number(payload.intensity);

    const response = await apiPost("/api/entries/add", payload);
    showMessage(response.message, response.ok ? "success" : "error");

    if (response.ok) {
      addEntryForm.reset();
      entryDateInput.value = getToday();
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
      await editEntry(entryId);
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

async function editEntry(entryId) {
  if (!ensureDiaryOpen()) {
    return;
  }

  const entry = entriesCache.find((item) => item.id === entryId);
  if (!entry) {
    showMessage("Memory not found in the current view.", "error");
    return;
  }

  const updatedTitle = prompt("Title", entry.title);
  if (updatedTitle === null) return;

  const updatedDescription = prompt("Description", entry.description);
  if (updatedDescription === null) return;

  const updatedIntensityInput = prompt("Intensity (0-10)", entry.intensity);
  if (updatedIntensityInput === null) return;
  const updatedIntensity = Number(updatedIntensityInput);
  if (Number.isNaN(updatedIntensity)) {
    showMessage("Intensity must be a number between 0 and 10.", "error");
    return;
  }

  const updatedDateInput = prompt("Date (YYYY-MM-DD)", formatDate(entry.date));
  if (updatedDateInput === null) return;

  const payload = {
    diaryName: currentDiary,
    password: currentPassword,
    entryId,
    title: updatedTitle.trim(),
    description: updatedDescription.trim(),
    intensity: updatedIntensity,
    date: updatedDateInput,
  };

  const response = await apiPost("/api/entries/update", payload);
  showMessage(response.message, response.ok ? "success" : "error");

  if (response.ok) {
    await openDiary(currentDiary, currentPassword, { silent: true });
  }
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
