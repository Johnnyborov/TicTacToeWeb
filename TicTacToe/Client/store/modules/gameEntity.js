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
      state.cells = Array(9).fill().map(i => ({status: 'clear', pressed: false}))
    },

    makePressed(state, index) {
      if (!state.gameOver && state.cells[index].status === 'clear')
        state.cells[index].pressed = true
    },

    makeUnpressed(state, index) {
      state.cells[index].pressed = false
    },

    makeMove(state, index) {
      if (state.movesCount%2 == 0) {
        state.cells[index].status = 'cross'
      } else if (state.movesCount%2 == 1) {
        state.cells[index].status = 'nought'
      }
      state.movesCount++
    },

    finishGame(state, foundChain) {
      if (foundChain && state.movesCount%2 == 1)
        state.winner = 'crosses'
      else if (foundChain && state.movesCount%2 == 0)
        state.winner = 'noughts'
      else
        state.winner = 'draw'
        
      state.gameOver = true
    }
  },

  actions: {
    tryMove({dispatch, commit, state}, index) {
      if (!state.gameOver && state.cells[index].status === 'clear') {
        commit('makeMove', index)
        dispatch('checkGame')
      }
    },

    checkGame({commit, state}) {
      // check rows
      for (let i = 0; i < 3; ++i) {
        let startStatus = state.cells[i*3].status
        if (startStatus !== 'clear') {
          let foundChain = true
          for (let k = 1; k < 3; ++k) {
            if (state.cells[i*3+k].status !== startStatus)
              foundChain = false
          }
          if (foundChain) {
            commit('finishGame', true)
            return
          }
        }
      }

      // check columns
      for (let j = 0; j < 3; ++j) {
        let startStatus = state.cells[j].status
        if (startStatus !== 'clear') {
          let found_chain = true
          for (let k = 1; k < 3; ++k) {
            if (state.cells[j+k*3].status !== startStatus)
              found_chain = false
          }
          if (found_chain) {
            commit('finishGame', true)
            return
          }
        }
      }

      // check dioganal right+down
      {
        let startStatus = state.cells[0].status
        if (startStatus !== 'clear') {
          let found_chain = true
          for (let k = 1; k < 3; ++k) {
            if (state.cells[k*3+k].status !== startStatus)
              found_chain = false
          }
          if (found_chain) {
            commit('finishGame', true)
            return
          }
        }
      }
      
      // check dioganal left+down
      {
        let startStatus = state.cells[2].status
        if (startStatus !== 'clear') {
          let found_chain = true
          for (let k = 1; k < 3; ++k) {
            if (state.cells[2+k*3-k].status !== startStatus)
              found_chain = false
          }
          if (found_chain) {
            commit('finishGame', true)
            return
          }
        }
      }

      if (state.movesCount === 9) {
        commit('finishGame', false)
        return
      }
    }
  }
}