async function register() {
    const name = document.getElementById("name").value;
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;
    const messageDiv = document.getElementById("message");
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");

    if (!name || !email || !password) {
        messageDiv.innerText = "Please fill in all fields.";
        messageDiv.className = "mt-3 text-center text-danger";
        return;
    }

    messageDiv.innerText = "Capturing face and registering...";
    messageDiv.className = "mt-3 text-center text-info";

    // Capture frame from video
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const context = canvas.getContext("2d");
    context.drawImage(video, 0, 0, canvas.width, canvas.height);
    
    // Convert to base64
    const image = canvas.toDataURL("image/jpeg");

    const payload = {
        fullName: name,
        email: email,
        password: password,
        role: "student",
        image: image
    };

    try {
        const response = await fetch("http://localhost:5242/api/auth/register", { // Adjust port if needed
            method: "POST",
            body: JSON.stringify(payload),
            headers: { "Content-Type": "application/json" }
        });

        const result = await response.json();

        if (response.ok) {
            messageDiv.innerText = "Success: " + (result.message || "User registered!");
            messageDiv.className = "mt-3 text-center text-success";
        } else {
            messageDiv.innerText = "Error: " + (result || "Registration failed.");
            messageDiv.className = "mt-3 text-center text-danger";
        }
    } catch (error) {
        console.error("Error:", error);
        messageDiv.innerText = "Error: Could not connect to the server.";
        messageDiv.className = "mt-3 text-center text-danger";
    }
}
