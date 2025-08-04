import api from 'src/services/api'

export const authService = {
  async login(email, password) {
    const response = await api.post('/User/Login', {
      userEmail: email,
      password: password
    })
    window.__treeNeedsReloading = true;
    return response.data
  },

  async register(name, email, password) {
    const response = await api.post('/User', {
      name: name,
      email: email,
      password: password,
      roles: ['User']
    })
    return response.data
  },

  async getAuthenticatedUser() {
    const response = await api.get('/User/Authenticated')
    return response.data
  },

  async getUserByEmail(email) {
    const response = await api.get(`/User/${email}`)
    return response.data
  },

  async changePassword(userId, userEmail, oldPassword, newPassword) {
    const response = await api.put('/User/ChangePassword', {
      userId: userId,
      userEmail: userEmail,
      oldPassword: oldPassword,
      newPassword: newPassword
    })
    return response.data
  },

  logout() {
    // Perform any client-side logout cleanup here, e.g., clearing tokens
    // This function is called when the user initiates a logout.
    window.__treeNeedsReloading = true;
  }
}
