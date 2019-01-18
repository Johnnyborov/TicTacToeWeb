function _newGame(state, isMyTurn) {
  if (isMyTurn)
    state.mySide = 'crosses'
  else
    state.mySide = 'noughts'

  state.isMyTurn = isMyTurn
  state.winByForfeit = false
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
  state.winner = conditions.winner

  state.gameOver = true

  setWinChainIndexes(state, conditions)

  if (conditions.direction === 'forfeit')
    state.winByForfeit = true
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
    isMyTurn: false,
    winByForfeit: false,
    
    mySide: '',
    movesCount: 0,
    winner: '',

    xDim: 10,
    yDim: 10,
    winSize: 4,

    winChainIndexes: [],
    cells: []
  },

  mutations: {
    newGame(state, {dimensions, isMyTurn}) {
      state.xDim = dimensions.xDim
      state.yDim = dimensions.yDim
      state.winSize = dimensions.winSize
      
      _newGame(state, isMyTurn)
    },

    makeMove(state, index) {
      setCellNewType(state, index)

      state.isMyTurn = !state.isMyTurn
    },

    finishGame(state, conditions) {
      _finishGame(state, conditions)
    }
  }
}