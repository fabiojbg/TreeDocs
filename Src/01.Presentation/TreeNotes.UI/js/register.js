$(function() {
  const registerForm = $('#registerForm');

  registerForm.on('submit', async function(e) {
    e.preventDefault();

    if (!this.checkValidity()) {
      e.stopPropagation();
      $(this).addClass('was-validated');
      return;
    }

    const password = $('#inputPassword').val();
    const passwordConfirm = $('#inputPasswordConfirm').val();
    if (password !== passwordConfirm) {
      toastr.error('Passwords do not match');
      $('#inputPasswordConfirm')[0].setCustomValidity('Passwords do not match');
      $(this).addClass('was-validated');
      return;
    } else {
      $('#inputPasswordConfirm')[0].setCustomValidity('');
    }

    $(this).removeClass('was-validated');

    const name = $('#inputName').val().trim();
    const email = $('#inputEmail').val().trim();

    try {
      const response = await fetch(`${API_BASE_URL}/api/User`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          Name: name,
          Email: email,
          Password: password,
          Roles: ['User']
        })
      });

      if (response.ok) {
        toastr.success('Registration successful! Please login.');
        setTimeout(() => {
          window.location.href = 'login.html';
        }, 1500);
      } else {
        const errData = await response.json();
        toastr.error(errData._Message || 'Registration failed');
      }
    } catch (err) {
      toastr.error('Network error during registration');
    }
  });
});
