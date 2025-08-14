$(function() {
  const loginForm = $('#loginForm');

  loginForm.on('submit', async function(e) {
    e.preventDefault();
    if (!this.checkValidity()) {
      e.stopPropagation();
      $(this).addClass('was-validated');
      return;
    }
    $(this).removeClass('was-validated');

    const email = $('#inputEmail').val().trim();
    const password = $('#inputPassword').val();

    try {
      const response = await fetch(`${API_BASE_URL}/api/User/Login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ UserEmail: email, Password: password })
      });

      if (response.ok) {
        const data = await response.json();
        // Store token and user info in localStorage for session management
        localStorage.setItem('authToken', data.token || data.Token);
        localStorage.setItem('userEmail', data.email || data.Email);
        localStorage.setItem('userName', data.userName || data.Username);
        toastr.success('Login successful');
        // Redirect to dashboard
        window.location.href = 'dashboard.html';
      } else {
        const errData = await response.json();
        toastr.error(errData._Message || 'Login failed');
      }
    } catch (err) {
      toastr.error('Network error during login');
    }
  });
});
