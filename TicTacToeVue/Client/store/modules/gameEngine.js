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

    makePressed(state, index) {
      state.cells[index].pressed = true
    },

    makeUnpressed(state, index) {
      state.cells[index].pressed = false
    },

    tryMove(state, index) {
      state.cells[index].type = 1
    }
  }
}