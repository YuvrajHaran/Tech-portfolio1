//// Section switching
//document.querySelectorAll('.nav-item[data-section]').forEach(item => {
//    item.addEventListener('click', function () {
//        document.querySelectorAll('.section').forEach(sec => {
//            sec.style.display = 'none';   // hide all sections completely
//        });
//        const activeSection = document.getElementById(this.dataset.section);
//        activeSection.style.display = 'block'; // show selected section

//        // Load projects if projects section is clicked
//        if (this.dataset.section === "projects") loadProjects();
//    });
//});

//// Initially show only home section
//document.querySelectorAll('.section').forEach(sec => sec.style.display = 'none');
//document.getElementById('home').style.display = 'block';

//// Load projects dynamically from API
//async function loadProjects() {
//    try {
//        const res = await fetch("https://localhost:7246/api/projects");
//        if (!res.ok) throw new Error("Failed to fetch projects");

//        const projects = await res.json();
//        const projectList = document.getElementById("project-list");
//        projectList.innerHTML = "";

//        projects.forEach(p => {
//            const div = document.createElement("div");
//            div.innerHTML = `<strong>${p.title}</strong>: ${p.description}`;
//            projectList.appendChild(div);
//        });
//    } catch (err) {
//        console.error(err);
//    }
//}


// Section switching
// Section switching 
document.querySelectorAll('.nav-item[data-section]').forEach(item => {
    item.addEventListener('click', function () {
        // Hide all sections
        document.querySelectorAll('.section').forEach(sec => sec.style.display = 'none');

        // Show the clicked section
        const activeSection = document.getElementById(this.dataset.section);
        if (activeSection) {
            activeSection.style.display = 'block';
        }

        // Load projects if projects section is clicked
        if (this.dataset.section === "projects") {
            loadProjects();
        }
    });
});

// Initially show only home section
document.querySelectorAll('.section').forEach(sec => sec.style.display = 'none');
const homeSection = document.getElementById('home');
if (homeSection) homeSection.style.display = 'block';

// Load projects dynamically from API
async function loadProjects() {
    try {
        const res = await fetch("https://localhost:7246/api/projects");
        if (!res.ok) throw new Error("Failed to fetch projects");

        const projects = await res.json();
        const projectList = document.getElementById("project-list");
        if (!projectList) return;

        // Clear existing
        projectList.innerHTML = "";

        // Add fetched projects
        projects.forEach(p => {
            const div = document.createElement("div");
            div.classList.add("project-card"); // optional styling class
            div.innerHTML = `
                <strong>${p.title}</strong>: ${p.description}<br/>
                ${p.githubLink ? `<a href="${p.githubLink}" target="_blank">GitHub</a><br/>` : ""}
                ${p.demoLink ? `<a href="${p.demoLink}" target="_blank">Demo</a><br/>` : ""}
            `;
            projectList.appendChild(div);
        });

        if (projects.length === 0) {
            projectList.innerHTML = "<p>No projects available right now.</p>";
        }
    } catch (err) {
        console.error("Error loading projects:", err);
        const projectList = document.getElementById("project-list");
        if (projectList) {
            projectList.innerHTML = `<p style="color:red;">⚠ Failed to load projects. Check API.</p>`;
        }
    }
}
