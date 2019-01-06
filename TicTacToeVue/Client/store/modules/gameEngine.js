export default {
  namespaced: true,
  state: {
    msg: 'gameEngine test data',
    cells: []
  },

  mutations: {
    fillDefault(state) {
      state.cells = Array(9).fill().map(i => ({type: 0, pressed: false}))
    },

    pressed(state, index) {
      state.cells[index].pressed = true
    },

    unPressed(state, index) {
      state.cells[index].pressed = false
    }
  }
}