import makeMove from './gameEntityMakeMoveImpl.js'

function resetGame(state) {
  state.gameOver = false
  state.movesCount = 0
  state.winner = ''
  state.cells = Array(state.xDim*state.yDim).fill().map(i => ({type: 'clear', status: 'normal'}))
}

export default {
  namespaced: true,
  
  state: {
    gameOver: false,
    movesCount: 0,
    winner: '',
    xDim: 3,
    yDim: 3,
    winSize: 3,
    cells: []
  },

  mutations: {
    fillDefault(state) {
      resetGame(state)
    },

    changeSizes(state, {xDim, yDim, winSize}) {
      state.xDim = xDim
      state.yDim = yDim
      state.winSize = winSize

      resetGame(state)
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