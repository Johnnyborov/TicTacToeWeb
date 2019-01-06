<template>
<div id="game-field">
  <canvas id="canvas" ref="canvas" @mousedown="mouseDown" @mouseup="mouseUp">
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
function getCellIndex(ctx, e) {
  let rect = ctx.canvas.getBoundingClientRect()
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

import Resources from '../resources/resources.js'
import FieldCell from './FieldCell.vue'

export default {
  data() {
    return {
      context: null,

      cross: null,
      nought: null,
      clear: null,

      lastPressed: -1
    }
  },

  components: {
    'field-cell': FieldCell
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
    mouseDown: function(e) {
      let index = getCellIndex(this.context, e)

      if (index > -1) {
        this.lastPressed = index
        this.$store.commit('gameEngine/pressed', index)
      }
    },

    mouseUp: function(e) {
      let index = getCellIndex(this.context, e)

      this.$store.commit('gameEngine/unPressed', this.lastPressed)
    },

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
