import makeMove from './gameEntityMakeMoveImpl.js'

const minXDim = 3
const minYDim = 3
const minWinSize = 2

const maxXDim = 100
const maxYDim = 100
const maxWinSize = 30

function resetGame(state) {
  state.gameOver = false
  state.movesCount = 0
  state.winner = ''
  state.cells = Array(state.xDim*state.yDim).fill().map(i => ({type: 'clear', status: 'normal'}))
}

export function validateDimensions(state, {xDim, yDim, winSize}) {
  if (xDim < minXDim || yDim < minYDim || winSize < minWinSize ||
    xDim > maxXDim || yDim > maxYDim || winSize > maxWinSize ||
    xDim === state.xDim && yDim === state.yDim && winSize === state.winSize ||
    minWinSize > Math.min(xDim,yDim))
    return false
  else
    return true
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