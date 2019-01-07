import makeMove from './gameEntityMakeMoveImpl.js'

export const dimLimits = {
  minXDim: 3,
  minYDim: 3,
  minWinSize: 2,
  maxXDim: 100,
  maxYDim: 100,
  maxWinSize: 30
}

export function validateDimensions(state, {xDim, yDim, winSize}) {
  if (xDim < dimLimits.minXDim || yDim < dimLimits.minYDim || winSize < dimLimits.minWinSize ||
      xDim > dimLimits.maxXDim || yDim > dimLimits.maxYDim || winSize > dimLimits.maxWinSize ||
      winSize > Math.min(xDim,yDim) ||
      xDim === state.xDim && yDim === state.yDim && winSize === state.winSize)
    return false
  else
    return true
}

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
    xDim: 25,
    yDim: 25,
    winSize: 5,
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