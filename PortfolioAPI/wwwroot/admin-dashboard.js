const apiBase = "https://localhost:7246/api/projects";
const token = localStorage.getItem("token");

if (!token) {
    alert("You must login first!");
    window.location.href = "admin-login.html";
}

// Fetch and show all projects
async function loadProjects() {
    try {
        const response = await fetch(apiBase, {
            headers: { "Authorization": `Bearer ${token}` }
        });
        const projects = await response.json();

        const container = document.getElementById("projects");
        container.innerHTML = "";
        projects.forEach(p => {
            const div = document.createElement("div");
            div.className = "project-item";
            div.innerHTML = `
                <h3>${p.title}</h3>
                <p>${p.description}</p>
                ${p.githubLink ? `<a href="${p.githubLink}" target="_blank">GitHub</a><br/>` : ""}
                ${p.demoLink ? `<a href="${p.demoLink}" target="_blank">Demo</a><br/>` : ""}
                <button class="action-btn" onclick="editProject(${p.projectId}, '${p.title}', '${p.description}', '${p.githubLink}', '${p.demoLink}')">Edit</button>
                <button class="action-btn" onclick="deleteProject(${p.projectId})">Delete</button>
            `;
            container.appendChild(div);
        });
    } catch (err) {
        console.error(err);
        alert("Error loading projects.");
    }
}

async function saveProject() {
    const id = document.getElementById("projectId").value;
    const title = document.getElementById("title").value.trim();
    const description = document.getElementById("description").value.trim();
    const githubLink = document.getElementById("githubLink").value.trim();
    const demoLink = document.getElementById("demoLink").value.trim();

    const project = { title, description, githubLink, demoLink };

    try {
        let response;
        if (id) {
            response = await fetch(`${apiBase}/${id}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(project)
            });
        } else {
            response = await fetch(apiBase, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(project)
            });
        }

        if (response.ok) {
            clearForm();
            loadProjects();
        } else {
            const errText = await response.text();
            alert("Error saving project: " + errText);
        }
    } catch (err) {
        console.error(err);
    }
}

function editProject(id, title, description, githubLink, demoLink) {
    document.getElementById("projectId").value = id;
    document.getElementById("title").value = title;
    document.getElementById("description").value = description;
    document.getElementById("githubLink").value = githubLink || "";
    document.getElementById("demoLink").value = demoLink || "";
}

async function deleteProject(id) {
    if (!confirm("Are you sure you want to delete this project?")) return;

    try {
        const response = await fetch(`${apiBase}/${id}`, {
            method: "DELETE",
            headers: { "Authorization": `Bearer ${token}` }
        });

        if (response.ok) {
            loadProjects();
        } else {
            const errText = await response.text();
            alert("Error deleting project: " + errText);
        }
    } catch (err) {
        console.error(err);
    }
}

function clearForm() {
    document.getElementById("projectId").value = "";
    document.getElementById("title").value = "";
    document.getElementById("description").value = "";
    document.getElementById("githubLink").value = "";
    document.getElementById("demoLink").value = "";
}

// Load projects initially
loadProjects();
