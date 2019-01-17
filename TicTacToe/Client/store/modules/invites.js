export default {
  namespaced: true,
  
  state: {
    avaliableOpponents: [],
    invites: [],

    selectedOpponentId: '',
    selectedInviteOpponentId: '',

    myPreferredDimensions: null
  },

  mutations: {
    reset(state){
      state.avaliableOpponents = []
      state.invites = []
      state.selectedOpponentId = ''
      state.selectedInviteOpponentId = ''
    },


    setAvaliableOpponents(state, ids){
      state.avaliableOpponents = ids
    },


    addInvite(state, invite){
      state.invites.push(invite)
    },

    deleteInvite(state, index){
      state.invites.splice(index, 1)
    },

    
    setSelectedOpponentId(state, id){
      state.selectedOpponentId = id
    },

    setSelectedInviteOpponentId(state, id){
      state.selectedInviteOpponentId = id
    },


    setMyPreferredDimensions(state, dimensions){
      state.myPreferredDimensions = dimensions
    }
  },

  actions: {
    initialize({commit, rootState}) {
      commit('setMyPreferredDimensions', {xDim: rootState.gameEntity.xDim, yDim: rootState.gameEntity.yDim, winSize: rootState.gameEntity.winSize})
    },

    
    updateAvaliablePlayers({state, commit}, {players, myId}) {
      commit('setAvaliableOpponents', players.filter(player=>player.key !== myId))

      if (!state.avaliableOpponents.find(player=> player.key === state.selectedOpponentId)) {
        commit('setSelectedOpponentId', '')
      }
    },

    removeInvite({state, commit}, id) {
      let index = state.invites.findIndex(invite => invite.id === id)
      if (index !== -1) {
        commit('deleteInvite', index)

        if (id == state.selectedInviteOpponentId)
          commit('setSelectedInviteOpponentId', '')
      }
    }
  }
}