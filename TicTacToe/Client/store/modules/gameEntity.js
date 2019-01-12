import GameChecker from './gameEntityGameChecker.js'

function _newGame(state) {
  state.gameOver = false
  state.movesCount = 0
  state.winner = ''
  state.winChainIndexes = []
  state.cells = Array(state.xDim*state.yDim).fill().map(i => 'clear')
}

function setCellNewType(state, index) {
  if (state.movesCount%2 == 0) {
    state.cells[index] = 'cross'
  } else if (state.movesCount%2 == 1) {
    state.cells[index] = 'nought'
  }
  state.movesCount++
}

function _finishGame(state, conditions) {
  if (conditions.direction !== 'draw' && state.movesCount%2 == 1) {
    state.winner = 'crosses'
  }
  else if (conditions.direction !== 'draw' && state.movesCount%2 == 0) {
    state.winner = 'noughts'
  }
  else {
    state.winner = 'draw'
  }

  state.gameOver = true

  setWinChainIndexes(state, conditions)
}

// winSize cells in one of 4 directions starting from cell (i,j)
function setWinChainIndexes(state, {direction, i, j}) {
  switch(direction) {
    case 'right':
      for (let k = 0; k < state.winSize; k++) 
        state.winChainIndexes.push(i*state.xDim + (j + k))
      break
    case 'down':
      for (let k = 0; k < state.winSize; k++) 
        state.winChainIndexes.push((i+k)*state.xDim + j)
      break
    case 'right+down':
      for (let k = 0; k < state.winSize; k++) 
        state.winChainIndexes.push((i+k)*state.xDim + (j + k))
      break
    case 'left+down':
      for (let k = 0; k < state.winSize; k++) 
        state.winChainIndexes.push((i+k)*state.xDim + (j - k))
      break
  }
}

export const dimLimits = {
  minXDim: 3,
  minYDim: 3,
  minWinSize: 2,
  maxXDim: 100,
  maxYDim: 100,
  maxWinSize: 30
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
    winChainIndexes: [],
    cells: []
  },

  mutations: {
    newGame(state) {
      _newGame(state)
    },

    changeSizes(state, {xDim, yDim, winSize}) {
      state.xDim = xDim
      state.yDim = yDim
      state.winSize = winSize
      
      _newGame(state)
    },

    makeMove(state, index) {
      setCellNewType(state, index)

      // let conditions = GameChecker.checkGame(state)
      // if (conditions.over) _finishGame(state, conditions)
    },

    finishGame(state, conditions) {
      _finishGame(state, conditions)
    }
  }
}