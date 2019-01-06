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
  for (let i = 0; i < 3; ++i) {
    let startType = state.cells[i*3].type
    if (startType !== 'clear') {
      let foundChain = true
      for (let k = 1; k < 3; ++k) {
        if (state.cells[i*3+k].type !== startType)
          foundChain = false
      }
      if (foundChain) {
        finishGame(state, {direction: 'right', index: i})
        return
      }
    }
  }

  // check columns
  for (let j = 0; j < 3; ++j) {
    let startType = state.cells[j].type
    if (startType !== 'clear') {
      let found_chain = true
      for (let k = 1; k < 3; ++k) {
        if (state.cells[j+k*3].type !== startType)
          found_chain = false
      }
      if (found_chain) {
        finishGame(state, {direction: 'down', index: j})
        return
      }
    }
  }

  // check dioganal right+down
  {
    let startType = state.cells[0].type
    if (startType !== 'clear') {
      let found_chain = true
      for (let k = 1; k < 3; ++k) {
        if (state.cells[k*3+k].type !== startType)
          found_chain = false
      }
      if (found_chain) {
        finishGame(state, {direction: 'right+down', index: 0})
        return
      }
    }
  }
  
  // check dioganal left+down
  {
    let startType = state.cells[2].type
    if (startType !== 'clear') {
      let found_chain = true
      for (let k = 1; k < 3; ++k) {
        if (state.cells[2+k*3-k].type !== startType)
          found_chain = false
      }
      if (found_chain) {
        finishGame(state, {direction: 'left+down', index: 2})
        return
      }
    }
  }

  if (state.movesCount === 9) {
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

function setGameOverStatuses(state, {direction, index}) {
  switch(direction) {
    case 'right':
      for (let k = 0; k < 3; ++k) 
        state.cells[index*3+k].status = 'chain-member'
      break
    case 'down':
      for (let k = 0; k < 3; ++k) 
        state.cells[index+k*3].status = 'chain-member'
      break
    case 'right+down':
      for (let k = 0; k < 3; ++k) 
        state.cells[k*3+k].status = 'chain-member'
      break
    case 'left+down':
      for (let k = 0; k < 3; ++k) 
        state.cells[2+k*3-k].status = 'chain-member'
      break
  }

  state.cells.forEach(cell => {
    if (cell.status !== 'chain-member')
      cell.status = 'disabled'
  });
}