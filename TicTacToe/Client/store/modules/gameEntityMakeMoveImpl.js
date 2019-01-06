export default makeMove

function makeMove(state, index) {
  if (state.movesCount%2 == 0) {
    state.cells[index].type = 'cross'
  } else if (state.movesCount%2 == 1) {
    state.cells[index].type = 'nought'
  }
  state.movesCount++

  checkGame(state)
}

function checkGame(state) {
  // check rows
  for (let i = 0; i < state.yDim; ++i) {
    for (let j = 0; j < state.xDim - state.winSize + 1; ++j) {

      let startType = state.cells[i*state.xDim + j].type
      if (startType !== 'clear') {

        let foundChain = true
        for (let k = 1; k < state.winSize; ++k) {
          if (state.cells[i*state.xDim + (j + k)].type !== startType)
            foundChain = false
        }

        if (foundChain) {
          finishGame(state, {direction: 'right', i: i, j: j})
          return
        }
      }
    }
  }

  // check columns
  for (let i = 0; i < state.yDim - state.winSize + 1; ++i) {
    for (let j = 0; j < state.xDim; ++j) {

      let startType = state.cells[i*state.xDim + j].type
      if (startType !== 'clear') {

        let found_chain = true
        for (let k = 1; k < state.winSize; ++k) {
          if (state.cells[(i+k)*state.xDim + j].type !== startType)
            found_chain = false
        }

        if (found_chain) {
          finishGame(state, {direction: 'down', i: i, j: j})
          return
        }
      }
    }
  }

  // check dioganal right+down
  for (let i = 0; i < state.yDim - state.winSize + 1; ++i) {
    for (let j = 0; j < state.xDim - state.winSize + 1; ++j) {

      let startType = state.cells[i*state.xDim + j].type
      if (startType !== 'clear') {

        let found_chain = true
        for (let k = 1; k < state.winSize; ++k) {
          if (state.cells[(i+k)*state.xDim + (j + k)].type !== startType)
            found_chain = false
        }

        if (found_chain) {
          finishGame(state, {direction: 'right+down', i: i, j: j})
          return
        }
      }
    }
  }
  
  // check dioganal left+down
  for (let i = 0; i < state.yDim - state.winSize + 1; ++i) {
    for (let j = state.winSize - 1; j < state.xDim; ++j) {

      let startType = state.cells[i*state.xDim + j].type
      if (startType !== 'clear') {

        let found_chain = true
        for (let k = 1; k < state.winSize; ++k) {
          if (state.cells[(i+k)*state.xDim + (j - k)].type !== startType)
            found_chain = false
        }

        if (found_chain) {
          finishGame(state, {direction: 'left+down', i: i, j: j})
          return
        }
      }
    }
  }

  if (state.movesCount === state.xDim * state.yDim) {
    finishGame(state, {direction: 'none', index: -1})
    return
  }
}

function finishGame(state, options) {
  if (options.direction !== 'none' && state.movesCount%2 == 1) {
    state.winner = 'crosses'
  }
  else if (options.direction !== 'none' && state.movesCount%2 == 0) {
    state.winner = 'noughts'
  }
  else {
    state.winner = 'draw'
  }
    
  state.gameOver = true

  setGameOverStatuses(state, options)
}

function setGameOverStatuses(state, {direction, i, j}) {
  state.cells = state.cells.map(val => {return {type: val.type, status: 'disabled'}})

  switch(direction) {
    case 'right':
      for (let k = 0; k < state.winSize; ++k) 
        state.cells[i*state.xDim + (j + k)].status = 'chain-member'
      break
    case 'down':
      for (let k = 0; k < state.winSize; ++k) 
        state.cells[(i+k)*state.xDim + j].status = 'chain-member'
      break
    case 'right+down':
      for (let k = 0; k < state.winSize; ++k) 
        state.cells[(i+k)*state.xDim + (j + k)].status = 'chain-member'
      break
    case 'left+down':
      for (let k = 0; k < state.winSize; ++k) 
        state.cells[(i+k)*state.xDim + (j - k)].status = 'chain-member'
      break
  }
}