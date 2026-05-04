async function register() {
    const name = document.getElementById("name").value;
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;
    const messageDiv = document.getElementById("message");
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");

    if (!name || !email || !password) {
        showToast("Validation Error", "Please fill in all fields.", "danger");
        return;
    }

    messageDiv.innerHTML = '<i class="fi fi-rr-loading me-2"></i> Capturing face and registering...';
    messageDiv.className = "mt-4 text-center text-primary";

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
        const response = await fetch("http://localhost:5242/api/auth/register", {
            method: "POST",
            body: JSON.stringify(payload),
            headers: { "Content-Type": "application/json" }
        });

        const result = await response.json();

        if (response.ok) {
            messageDiv.innerHTML = '<i class="fi fi-rr-check-circle me-2"></i> Registration Complete';
            messageDiv.className = "mt-4 text-center text-success";
            
            showModal(
                "Registration Successful", 
                `<div class="text-center py-4">
                    <i class="fi fi-rr-badge-check text-success" style="font-size: 5rem;"></i>
                    <h3 class="mt-4 fw-bold">Welcome, ${name}!</h3>
                    <p class="text-muted">Your biometric ID has been successfully enrolled in the system.</p>
                </div>`,
                () => { window.location.href = "/Index"; }
            );
        } else {
            const errorMsg = result.message || result || "Registration failed.";
            showToast("Registration Error", errorMsg, "danger");
            messageDiv.innerHTML = '<i class="fi fi-rr-cross-circle me-2"></i> ' + errorMsg;
            messageDiv.className = "mt-4 text-center text-danger";
        }
    } catch (error) {
        console.error("Error:", error);
        showToast("Connection Error", "Could not connect to the server.", "danger");
        messageDiv.innerHTML = '<i class="fi fi-rr-wifi-slash me-2"></i> Connection failed';
        messageDiv.className = "mt-4 text-center text-danger";
    }
}

