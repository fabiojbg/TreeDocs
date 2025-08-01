import api from 'src/services/api'

export const nodeService = {
  async getUserNodes() {
    const response = await api.get('/v1/nodes')
    return response.data
  },

  async getNodeById(nodeId) {
    const response = await api.get(`/v1/nodes/${nodeId}`)
    return response.data
  },

  async createNode(parentId, name, nodeType, contents) {
    const response = await api.post('/v1/nodes', {
      parentId: parentId,
      name: name,
      nodeType: nodeType,
      contents: contents
    })
    return response.data
  },

  async updateNode(nodeId, parentId, name, nodeContents, childrenOrder) {
    const response = await api.put('/v1/nodes', {
      nodeId: nodeId,
      parentId: parentId,
      name: name,
      nodeContents: nodeContents,
      childrenOrder: childrenOrder
    })
    return response.data
  },

  async deleteNode(nodeId) {
    const response = await api.delete(`/v1/nodes/${nodeId}`)
    return response.data
  }
}
