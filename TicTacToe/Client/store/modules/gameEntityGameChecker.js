export default {
  // ckeck for chain=|winSize cells of same type in a row| in 4 possible directions
  // for every valid starting point on the field
  checkGame(state) {
    // check rows
    for (let i = 0; i < state.yDim; i++) {
      for (let j = 0; j < state.xDim - state.winSize + 1; j++) {
  
        let start = state.cells[i*state.xDim + j]
        if (start !== 'clear') {
  
          let foundChain = true
          for (let k = 1; k < state.winSize; k++) {
            if (state.cells[i*state.xDim + (j + k)] !== start)
              foundChain = false
          }
  
          if (foundChain) {
            return {over: true, direction: 'right', i: i, j: j}
          }
        }
      }
    }
  
    // check columns
    for (let i = 0; i < state.yDim - state.winSize + 1; i++) {
      for (let j = 0; j < state.xDim; j++) {
  
        let start = state.cells[i*state.xDim + j]
        if (start !== 'clear') {
  
          let foundChain = true
          for (let k = 1; k < state.winSize; k++) {
            if (state.cells[(i+k)*state.xDim + j] !== start)
              foundChain = false
          }
  
          if (foundChain) {
            return {over: true, direction: 'down', i: i, j: j}
          }
        }
      }
    }
  
    // check dioganal right+down
    for (let i = 0; i < state.yDim - state.winSize + 1; i++) {
      for (let j = 0; j < state.xDim - state.winSize + 1; j++) {
  
        let start = state.cells[i*state.xDim + j]
        if (start !== 'clear') {
  
          let foundChain = true
          for (let k = 1; k < state.winSize; k++) {
            if (state.cells[(i+k)*state.xDim + (j + k)] !== start)
              foundChain = false
          }
  
          if (foundChain) {
            return {over: true, direction: 'right+down', i: i, j: j}
          }
        }
      }
    }
    
    // check dioganal left+down
    for (let i = 0; i < state.yDim - state.winSize + 1; i++) {
      for (let j = state.winSize - 1; j < state.xDim; j++) {
  
        let start = state.cells[i*state.xDim + j]
        if (start !== 'clear') {
  
          let foundChain = true
          for (let k = 1; k < state.winSize; k++) {
            if (state.cells[(i+k)*state.xDim + (j - k)] !== start)
              foundChain = false
          }
  
          if (foundChain) {
            return {over: true, direction: 'left+down', i: i, j: j}
          }
        }
      }
    }
  
    if (state.movesCount === state.xDim * state.yDim) {
      return {over: true, direction: 'draw', i: null, j: null}
    }

    return {over: false, direction: null, i: null, j: null}
  }
}
