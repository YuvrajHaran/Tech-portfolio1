async function login() {
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();

    try {
        const response = await fetch("https://localhost:7246/api/admin/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password })
        });

        if (response.ok) {
            const data = await response.json();
            localStorage.setItem("token", data.token);
            window.location.href = "admin-dashboard.html";
        } else {
            document.getElementById("error").textContent = "Invalid credentials!";
        }
    } catch (err) {
        console.error(err);
        document.getElementById("error").textContent = "Server error!";
    }
}
