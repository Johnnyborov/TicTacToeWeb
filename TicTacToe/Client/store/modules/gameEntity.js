import makeMove from './gameEntityMakeMoveImpl.js'

export default {
  namespaced: true,
  
  state: {
    gameOver: false,
    movesCount: 0,
    winner: '',
    cells: []
  },

  mutations: {
    fillDefault(state) {
      state.gameOver = false
      state.movesCount = 0
      state.winner = ''
      state.cells = Array(9).fill().map(i => ({type: 'clear', status: 'normal'}))
    },

    setPressed(state, index) {
      if (!state.gameOver && state.cells[index].type === 'clear')
        state.cells[index].status = 'pressed'
    },

    setUnpressed(state, index) {
      if (state.cells[index].status === 'pressed')
        state.cells[index].status = 'normal'
    },

    tryMove(state, index) {
      if (!state.gameOver && state.cells[index].type === 'clear') {
        makeMove(state, index)
      }
    }
  }
}