<template>
<div id="game-field">
  <canvas id="canvas" ref="canvas"
  @mousedown.prevent="$emit('mouse-down', getCellIndex($event))"
  @mouseup="$emit('mouse-up', getCellIndex($event))"
  @mouseleave="$emit('mouse-leave')">
  </canvas>
</div>
</template>

<script>
import Resources from '../resources/resources.js'

export default {
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
      this.context.clearRect(0,0,this.context.canvas.width,this.context.canvas.height)

      for (let index = 0; index < this.$store.state.gameEntity.cells.length; index++) {
        this.drawCell(index)
      }
    },

    drawCell(index) {
      if (!this.context) return
      let ctx = this.context

      let xDim = this.$store.state.gameEntity.xDim
      let yDim = this.$store.state.gameEntity.yDim
      let maxDim = Math.max(xDim,yDim)

      let cellBorderWidth = Math.floor(ctx.canvas.width / 12 / maxDim)
      if (cellBorderWidth === 0) {
        cellBorderWidth = 1
      }
      let imgSize = Math.floor((ctx.canvas.width - (maxDim*2 + maxDim-1)*cellBorderWidth) / maxDim)
      let imgStep = (imgSize + 3*cellBorderWidth)


      let i = Math.floor(index / this.$store.state.gameEntity.xDim)
      let j = index % this.$store.state.gameEntity.xDim

      let value = this.$store.state.gameEntity.cells[index]

      if (value.status == 'pressed')
        ctx.strokeStyle = '#08F'
      else if  (value.status == 'disabled')
        ctx.strokeStyle = '#444'
      else if  (value.status == 'chain-member')
        ctx.strokeStyle = '#080'
      else if  (value.status == 'normal')
        ctx.strokeStyle = '#048'
      else
        throw "invalid cell status"

      ctx.lineWidth = cellBorderWidth

      ctx.strokeRect(cellBorderWidth/2+imgStep*j, cellBorderWidth/2+imgStep*i, imgSize+cellBorderWidth, imgSize+cellBorderWidth)

      let cellImg;
      switch(value.type) {
        case 'clear':
          cellImg = this.clearImg
          break
        case 'cross':
          cellImg = this.crossImg
          break
        case 'nought':
          cellImg = this.noughtImg
          break
        default:
          throw "Invalid cell type"
      }

      ctx.drawImage(cellImg, cellBorderWidth+imgStep*j, cellBorderWidth+imgStep*i, imgSize, imgSize)
    },

    getCellIndex: function(e) {
      let xDim = this.$store.state.gameEntity.xDim
      let yDim = this.$store.state.gameEntity.yDim
      let maxDim = Math.max(xDim,yDim)

      let halfCellBorderWidth = Math.floor(this.context.canvas.width / 25 / maxDim)
      let cellBorderWidth = 2 * halfCellBorderWidth
      if (halfCellBorderWidth === 0) {
        cellBorderWidth = 1
      }
      let imgSize = Math.floor((this.context.canvas.width - maxDim*3*cellBorderWidth) / maxDim)
      let imgStep = (imgSize + 3*cellBorderWidth)


      let rect = this.context.canvas.getBoundingClientRect()
      let x = e.clientX - rect.left
      let y = e.clientY - rect.top

      let i = Math.floor(y / imgStep)
      let j = Math.floor(x / imgStep)

      if (i >= yDim || j >= xDim)
        return -1

      if (y%imgStep < halfCellBorderWidth || (imgStep - y%imgStep) < halfCellBorderWidth ||
          x%imgStep < halfCellBorderWidth || (imgStep - x%imgStep) < halfCellBorderWidth)
          return -1

      return i*xDim + j
    }
  }
}
</script>

<style>
  #game-field {
    background: sienna;
    width: 100%;
    height: 70%;
    position: absolute;
  }

  @media (orientation: landscape) {
    #game-field {
      width: 70%;
      height: 100%;
    }
  }
</style>
