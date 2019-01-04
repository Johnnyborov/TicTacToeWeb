import Vue from 'vue'
import Vuex from 'vuex'
import movesValidator from './modules/movesValidator.js'
import gameEngine from './modules/gameEngine.js'


Vue.use(Vuex)

const debug = process.env.NODE_ENV !== 'production'

export default new Vuex.Store({
  strict: debug,
  modules: {
    movesValidator,
    gameEngine
  }
})