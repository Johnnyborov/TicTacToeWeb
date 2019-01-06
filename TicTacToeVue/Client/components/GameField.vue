<template>
<div id="game-field">
  <canvas id="canvas" ref="canvas"
  @mousedown.prevent="$emit('mouse-down', getCellIndex($event))"
  @mouseup="$emit('mouse-up', getCellIndex($event))"
  @mouseleave="$emit('mouse-leave')">
    <field-cell
      v-for="(cell,index) in $store.state.gameEngine.cells"
      :value="cell"
      :i="Math.floor(index / 3)"
      :j="index % 3"
      :key="index">
    </field-cell>
  </canvas>
</div>
</template>

<script>
import Resources from '../resources/resources.js'
import FieldCell from './FieldCell.vue'

export default {
  components: {
    'field-cell': FieldCell
  },

  data() {
    return {
      context: null,

      clearImg: null,
      crossImg: null,
      noughtImg: null,
    }
  },

  created() {
    window.addEventListener("resize", this.resizeHandler)
    window.addEventListener('load', this.loadedHandler)
  },
  
  destroyed() {
    window.removeEventListener("resize", this.resizeHandler)
    window.removeEventListener('load', this.loadedHandler)
  },

  mounted() {
    this.context = this.$refs['canvas'].getContext('2d')

    this.$refs['canvas'].width = this.$refs['canvas'].parentElement.clientWidth
    this.$refs['canvas'].height = this.$refs['canvas'].parentElement.clientHeight

    this.clearImg = new Image()
    this.clearImg.src = Resources.clear

    this.crossImg = new Image()
    this.crossImg.src = Resources.cross

    this.noughtImg = new Image()
    this.noughtImg.src = Resources.nought
  },

  methods: { 
    resizeHandler: function() {
      this.$refs['canvas'].width = this.$refs['canvas'].parentElement.clientWidth
      this.$refs['canvas'].height = this.$refs['canvas'].parentElement.clientHeight

      this.drawAllCells()
    },

    loadedHandler: function() {
      this.drawAllCells()
    },

    drawAllCells: function() {
      this.$children.forEach(child => {
        child.$forceUpdate()
      });
    },

    getCellIndex: function(e) {
      let rect = this.context.canvas.getBoundingClientRect()
      let x = e.clientX - rect.left
      let y = e.clientY - rect.top

      let cellWithInterspace = rect.height / 3
      let i = Math.floor(y / cellWithInterspace)
      let j = Math.floor(x / cellWithInterspace)

      let halfCellBorderWidth = Math.round(rect.height / 40 / 2)
      if (y%cellWithInterspace < halfCellBorderWidth || (cellWithInterspace - y%cellWithInterspace) < halfCellBorderWidth ||
          x%cellWithInterspace < halfCellBorderWidth || (cellWithInterspace - x%cellWithInterspace) < halfCellBorderWidth)
          return -1

      return i*3 + j
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
