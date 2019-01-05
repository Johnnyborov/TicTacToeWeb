<template>
<div id="game-field">
  <canvas id="canvas" ref="canvas">
    <cells-renderer></cells-renderer>
  </canvas>
</div>
</template>

<script>
import Resources from '../resources/resources.js'
import CellsRenderer from './CellsRenderer.vue'

export default {
  data() {
    return {
      message: 'Basic Store Project Template',
      context: null,
      cross: null,
      nought: null,
      clear: null
    }
  },

  components: {
    'cells-renderer': CellsRenderer
  },

  created() {
    window.addEventListener("resize", this.resizeHandler)
    window.addEventListener('load', this.ready)
  },
  
  destroyed() {
    window.removeEventListener("resize", this.resizeHandler)
    window.removeEventListener('load', this.ready)
  },

  mounted() {
    this.context = this.$refs['canvas'].getContext('2d')

    this.$refs['canvas'].width = this.$refs['canvas'].parentElement.clientWidth
    this.$refs['canvas'].height = this.$refs['canvas'].parentElement.clientHeight

    var cross = new Image()
    cross.src = Resources.cross
    this.cross = cross

    var nought = new Image()
    nought.src = Resources.nought
    this.nought = nought

    var clear = new Image()
    clear.src = Resources.clear
    this.clear = clear
  },

  methods: {
    resizeHandler: function() {
      this.$refs['canvas'].width = this.$refs['canvas'].parentElement.clientWidth
      this.$refs['canvas'].height = this.$refs['canvas'].parentElement.clientHeight

      this.$children.forEach(child => {
        child.$forceUpdate();
      });
    },

    ready: function() {
      this.$children.forEach(child => {
        child.$forceUpdate();
      });
    }
  }
}
</script>

<style>
  #game-field {
    background: sienna;
    width: 66%;
    height: 100%;
    position: absolute;
  }
</style>
