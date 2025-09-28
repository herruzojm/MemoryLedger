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
const openModal = document.getElementById("open-modal");
const openModalCloseButton = document.getElementById("open-close");
const openModalCancelButton = document.getElementById("open-cancel");
const createModal = document.getElementById("create-modal");
const createModalCloseButton = document.getElementById("create-close");
const createModalCancelButton = document.getElementById("create-cancel");
const createDiaryButton = document.getElementById("create-diary-button");
const deleteDiaryButton = document.getElementById("delete-diary");
const refreshButton = document.getElementById("refresh-diary");
const closeButton = document.getElementById("close-diary");
const addEntryButton = document.getElementById("add-entry");
const searchForm = document.getElementById("search-form");
const resetSearchButton = document.getElementById("reset-search");
const averageForm = document.getElementById("average-form");
const averageStartInput = document.getElementById("average-start");
const averageEndInput = document.getElementById("average-end");
const statisticsButton = document.getElementById("statistics-button");
const entryModal = document.getElementById("entry-modal");
const entryModalTitle = document.getElementById("entry-modal-title");
const entryForm = document.getElementById("entry-form");
const entrySubmitButton = document.getElementById("entry-submit");
const entryCancelButton = document.getElementById("entry-cancel");
const entryCloseButton = document.getElementById("entry-close");
const statsModal = document.getElementById("stats-modal");
const statsCloseButton = document.getElementById("stats-close");
const statsAverageText = document.getElementById("stats-average");
const statsChartWrapper = document.getElementById("stats-chart-wrapper");
const statsCanvas = document.getElementById("stats-chart");
const statsEmpty = document.getElementById("stats-empty");

const entryDateInput = document.getElementById("entry-date");
const entryTitleInput = document.getElementById("entry-title");
const entryDescriptionInput = document.getElementById("entry-description");
const entryIntensityInput = document.getElementById("entry-intensity");

let currentDiary = null;
let currentPassword = null;
let entriesCache = [];
let editingEntryId = null;
let statsChartInstance = null;
let chartLibrary = null;

const getToday = () => new Date().toISOString().slice(0, 10);
entryDateInput.value = getToday();

init();

function init() {
  loadDiaries();

  if (statisticsButton) {
    statisticsButton.disabled = true;
  }

  if (createDiaryButton) {
    createDiaryButton.addEventListener("click", () => {
      openCreateModal();
    });
  }

  createModalCloseButton.addEventListener("click", () => {
    closeCreateModal();
  });

  createModalCancelButton.addEventListener("click", () => {
    closeCreateModal();
  });

  createModal.addEventListener("click", (event) => {
    if (event.target === createModal) {
      closeCreateModal();
    }
  });

  openModalCloseButton.addEventListener("click", () => {
    closeOpenModal();
  });

  openModalCancelButton.addEventListener("click", () => {
    closeOpenModal();
  });

  openModal.addEventListener("click", (event) => {
    if (event.target === openModal) {
      closeOpenModal();
    }
  });

  createForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const formData = new FormData(createForm);
    const payload = Object.fromEntries(formData.entries());

    const response = await apiPost("/api/diaries/create", payload);
    showMessage(response.message, response.ok ? "success" : "error");

    if (response.ok) {
      createForm.reset();
      closeCreateModal();
      loadDiaries();
    }
  });

  openForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    await openDiaryFromForm();
  });

  deleteDiaryButton.addEventListener("click", async () => {
    if (!ensureDiaryOpen()) {
      return;
    }

    if (!confirm(`Delete the diary "${currentDiary}"? This action cannot be undone.`)) {
      return;
    }

    const response = await apiPost("/api/diaries/delete", {
      name: currentDiary,
      password: currentPassword,
    });
    showMessage(response.message, response.ok ? "success" : "error");

    if (response.ok) {
      closeDiaryView();
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
    if (event.key !== "Escape") {
      return;
    }

    if (isCreateModalOpen()) {
      closeCreateModal();
    } else if (isOpenModalOpen()) {
      closeOpenModal();
    } else if (isStatsModalOpen()) {
      closeStatisticsModal();
    } else if (!entryModal.classList.contains("hidden")) {
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
    averageForm.reset();
    renderEntries(entriesCache);
  });

  averageForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (!ensureDiaryOpen()) {
      return;
    }

    const stats = collectStatistics();
    updateAverageSummary(stats);
    updateStatisticsButtonState(stats);
    if (statisticsButton.disabled) {
      return;
    }

    if (isStatsModalOpen()) {
      await renderStatisticsChart(stats);
      return;
    }

    await openStatisticsModal();
  });

  [averageStartInput, averageEndInput].forEach((input) => {
    input.addEventListener("input", () => {
      if (!ensureDiaryOpen()) {
        return;
      }

      const stats = collectStatistics();
      updateAverageSummary(stats);
      updateStatisticsButtonState(stats);
      if (isStatsModalOpen()) {
        renderStatisticsChart(stats);
      }
    });
  });

  statisticsButton.addEventListener("click", async () => {
    if (!ensureDiaryOpen()) {
      return;
    }

    await openStatisticsModal();
  });

  statsCloseButton.addEventListener("click", () => {
    closeStatisticsModal();
  });

  statsModal.addEventListener("click", (event) => {
    if (event.target === statsModal) {
      closeStatisticsModal();
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

function openCreateModal() {
  createForm.reset();
  createModal.classList.remove("hidden");
  document.body.classList.add("modal-open");
  createForm.name.focus();
}

function closeCreateModal() {
  if (createModal.classList.contains("hidden")) {
    return;
  }

  createModal.classList.add("hidden");
  createForm.reset();
  if (!isAnyModalOpen()) {
    document.body.classList.remove("modal-open");
  }
}

function isCreateModalOpen() {
  return !createModal.classList.contains("hidden");
}

function openDiaryModal(presetName = "") {
  openForm.reset();
  const normalizedName = presetName ?? "";
  if (normalizedName) {
    openForm.name.value = normalizedName;
  }
  openModal.classList.remove("hidden");
  document.body.classList.add("modal-open");
  const focusTarget = normalizedName ? openForm.password : openForm.name;
  focusTarget.focus();
}

function closeOpenModal() {
  if (openModal.classList.contains("hidden")) {
    return;
  }

  openModal.classList.add("hidden");
  openForm.reset();
  if (!isAnyModalOpen()) {
    document.body.classList.remove("modal-open");
  }
}

function isOpenModalOpen() {
  return !openModal.classList.contains("hidden");
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
  averageForm.reset();
  renderEntries(entriesCache);
  diaryView.classList.remove("hidden");
  deleteDiaryButton.classList.remove("hidden");
  closeOpenModal();
}

function closeDiaryView() {
  currentDiary = null;
  currentPassword = null;
  entriesCache = [];
  diaryTitle.textContent = "";
  diarySubtitle.textContent = "";
  averageForm.reset();
  if (averageResult) {
    averageResult.textContent = "";
  }
  if (statsAverageText) {
    statsAverageText.textContent = "";
  }
  if (statisticsButton) {
    statisticsButton.disabled = true;
    statisticsButton.removeAttribute("title");
  }
  closeStatisticsModal();
  statsChartWrapper.classList.add("hidden");
  statsEmpty.classList.add("hidden");
  statsEmpty.textContent = "";
  entriesContainer.innerHTML = "";
  entriesEmpty.classList.remove("hidden");
  diaryView.classList.add("hidden");
  deleteDiaryButton.classList.add("hidden");
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
  entryForm.reset();
  entryDateInput.value = getToday();
  entryIntensityInput.value = 5;
  if (!isAnyModalOpen()) {
    document.body.classList.remove("modal-open");
  }
}

function renderEntries(entries, options = {}) {
  entriesContainer.innerHTML = "";

  const list = Array.isArray(entries) ? entries : [];
  if (!options.fromSearch) {
    entriesCache = list;
    const stats = collectStatistics();
    updateAverageSummary(stats);
    updateStatisticsButtonState(stats);
    if (isStatsModalOpen()) {
      renderStatisticsChart(stats);
    }
  }

  if (list.length === 0) {
    entriesEmpty.classList.remove("hidden");
    return;
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

function collectStatistics() {
  const start = averageStartInput.value || "";
  const end = averageEndInput.value || "";
  const hasEntries = entriesCache.length > 0;
  const invalidRange = Boolean(start && end && start > end);

  if (!hasEntries) {
    return { start, end, invalidRange, entries: [], average: null, hasEntries };
  }

  const normalizedEntries = entriesCache
    .map((entry) => {
      const normalizedDate = (formatDate(entry.date) || "").slice(0, 10);
      if (!normalizedDate) {
        return null;
      }
      return { ...entry, normalizedDate };
    })
    .filter(Boolean);

  const filtered = normalizedEntries.filter((entry) => {
    if (start && entry.normalizedDate < start) return false;
    if (end && entry.normalizedDate > end) return false;
    return true;
  });

  filtered.sort((a, b) => a.normalizedDate.localeCompare(b.normalizedDate));

  const average =
    filtered.length > 0
      ? filtered.reduce((sum, entry) => sum + Number(entry.intensity ?? 0), 0) / filtered.length
      : null;

  return { start, end, invalidRange, entries: filtered, average, hasEntries };
}

function describeRange({ start, end }) {
  if (start && end) {
    return `from ${formatDate(start)} to ${formatDate(end)}`;
  }
  if (start) {
    return `from ${formatDate(start)} onward`;
  }
  if (end) {
    return `up to ${formatDate(end)}`;
  }
  return "for all memories";
}

function updateAverageSummary(stats = collectStatistics()) {
  if (!currentDiary) {
    if (averageResult) {
      averageResult.textContent = "";
    }
    if (statsAverageText) {
      statsAverageText.textContent = "";
    }
    return stats;
  }

  if (!stats.hasEntries) {
    const message = "No memories recorded yet.";
    if (averageResult) {
      averageResult.textContent = message;
    }
    if (statsAverageText) {
      statsAverageText.textContent = message;
    }
    return stats;
  }

  if (stats.invalidRange) {
    const message = "Start date cannot be after end date.";
    if (averageResult) {
      averageResult.textContent = message;
    }
    if (statsAverageText) {
      statsAverageText.textContent = message;
    }
    return stats;
  }

  if (stats.entries.length === 0) {
    const message = "No memories in the selected range.";
    if (averageResult) {
      averageResult.textContent = message;
    }
    if (statsAverageText) {
      statsAverageText.textContent = message;
    }
    return stats;
  }

  const averageValue = Number(stats.average).toFixed(2);
  const summary = `Average intensity ${describeRange(stats)}: ${averageValue}`;
  if (averageResult) {
    averageResult.textContent = summary;
  }
  if (statsAverageText) {
    statsAverageText.textContent = summary;
  }
  return stats;
}

function updateStatisticsButtonState(stats = collectStatistics()) {
  if (!statisticsButton) {
    return stats;
  }

  const disabled = !currentDiary || !stats.hasEntries || stats.invalidRange;
  statisticsButton.disabled = disabled;

  if (disabled && stats.invalidRange) {
    statisticsButton.title = "Select a valid period (start date cannot be after end date).";
  } else if (disabled && !stats.hasEntries) {
    statisticsButton.title = "Add memories to view statistics.";
  } else {
    statisticsButton.removeAttribute("title");
  }

  return stats;
}

function isStatsModalOpen() {
  return !statsModal.classList.contains("hidden");
}

function isAnyModalOpen() {
  return (
    isCreateModalOpen() ||
    isOpenModalOpen() ||
    !entryModal.classList.contains("hidden") ||
    isStatsModalOpen()
  );
}

async function openStatisticsModal() {
  statsModal.classList.remove("hidden");
  document.body.classList.add("modal-open");
  await renderStatisticsChart();
}

function closeStatisticsModal() {
  if (statsModal.classList.contains("hidden")) {
    return;
  }

  statsModal.classList.add("hidden");
  statsEmpty.classList.add("hidden");
  statsEmpty.textContent = "";
  if (!isAnyModalOpen()) {
    document.body.classList.remove("modal-open");
  }
}

async function renderStatisticsChart(statsData) {
  try {
    const stats = statsData ?? collectStatistics();
    updateAverageSummary(stats);
    updateStatisticsButtonState(stats);

    if (!stats.hasEntries) {
      showStatsMessage("No memories recorded yet.");
      return;
    }

    if (stats.invalidRange) {
      showStatsMessage("Start date cannot be after end date.");
      return;
    }

    if (stats.entries.length === 0) {
      showStatsMessage("No memories in the selected range.");
      return;
    }

    const Chart = await loadChartLibrary();
    if (!Chart) {
      showStatsMessage("Unable to load the chart library.");
      return;
    }

    showStatsChart();

    const labels = stats.entries.map((entry) => entry.normalizedDate);
    const intensities = stats.entries.map((entry) => Number(entry.intensity ?? 0));
    const averageFixed = Number(stats.average.toFixed(2));
    const averageData = labels.map(() => averageFixed);

    if (!statsChartInstance) {
      const context = statsCanvas.getContext("2d");
      if (!context) {
        showStatsMessage("Unable to render the chart.");
        return;
      }

      statsChartInstance = new Chart(context, {
        type: "line",
        data: {
          labels,
          datasets: [
            {
              label: "Intensity",
              data: intensities,
              borderColor: "#2563eb",
              backgroundColor: "rgba(37, 99, 235, 0.15)",
              tension: 0.3,
              fill: false,
              pointBackgroundColor: "#2563eb",
              pointRadius: 4,
              pointHoverRadius: 6,
            },
            {
              label: "Average",
              data: averageData,
              borderColor: "#f97316",
              borderDash: [6, 6],
              borderWidth: 2,
              fill: false,
              pointRadius: 0,
              pointHoverRadius: 0,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          interaction: { intersect: false, mode: "index" },
          scales: {
            y: {
              beginAtZero: true,
              suggestedMin: 0,
              suggestedMax: 10,
              title: { display: true, text: "Intensity" },
            },
            x: {
              title: { display: true, text: "Date" },
            },
          },
          plugins: {
            legend: {
              labels: { usePointStyle: true },
            },
            tooltip: {
              callbacks: {
                label(context) {
                  if (context.datasetIndex === 1) {
                    return `Average: ${Number(context.parsed.y).toFixed(2)}`;
                  }
                  return `Intensity: ${context.parsed.y}`;
                },
              },
            },
          },
        },
      });
    } else {
      statsChartInstance.data.labels = labels;
      statsChartInstance.data.datasets[0].data = intensities;
      statsChartInstance.data.datasets[1].data = averageData;
      statsChartInstance.update();
    }
  } catch (error) {
    console.error(error);
    showStatsMessage("Unable to render the chart.");
  }
}

async function loadChartLibrary() {
  if (chartLibrary) {
    return chartLibrary;
  }

  if (globalThis.Chart) {
    chartLibrary = globalThis.Chart;
    return chartLibrary;
  }

  try {
    const module = await import("https://cdn.jsdelivr.net/npm/chart.js@4.4.5/dist/chart.umd.min.js");
    chartLibrary = module?.Chart || module?.default || globalThis.Chart;
    return chartLibrary;
  } catch (error) {
    console.error("Failed to load chart library", error);
    showMessage("Unable to load the chart library.", "error");
    return null;
  }
}

function showStatsChart() {
  statsChartWrapper.classList.remove("hidden");
  statsEmpty.classList.add("hidden");
}

function showStatsMessage(message) {
  statsChartWrapper.classList.add("hidden");
  statsEmpty.textContent = message;
  statsEmpty.classList.remove("hidden");
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
      openDiaryModal(name);
    });
    item.addEventListener("keydown", (event) => {
      if (event.key === "Enter" || event.key === " ") {
        event.preventDefault();
        openDiaryModal(name);
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
