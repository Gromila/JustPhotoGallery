(function($){
/*
	Script: jQuery.xdCloudTags.js
	Plug-in for jQuery to transform an ordinary tag cloud to the cloud with vertical and horizontal tags
*/

/*
	An algorithm implementation in JavaScript for rectangle packing.
	Author:
		Valeriy Chupurnov <leroy@xdan.ru>, <http://xdan.ru>
	License:
		LGPL - Lesser General Public License
	Class: xdRectPacker
	A class that finds an 'efficient' position for a rectangle inside another rectangle
	without overlapping the space already taken.
	
	Posted <http://xdan.ru/2d-bin-packing-with-javascript.html>
	
	A simple search of the empty positions, so it works very slowly.
	Warning: not recommended for more than 50 objects
*/
var xdRectPacker = function ( sideSize,horizontal ){
	this.side = (!horizontal)?'width':'height';
	this[this.side] = parseInt(sideSize);
	this._y = (this.side=='width')?'y':'x';
	this._x = (this.side=='width')?'x':'y';
	this._w = (this.side=='width')?'w':'h';
};
var xdRect = function(x,y,w,h){
  return {
    x:x,
    y:y,
    w:w,
    h:h,
    x1:function(){ return this.x+this.w;},
    y1:function(){ return this.y+this.h;},
    intersect:function( rc ){
      return (
			(
				(
					( rc.x>=this.x && rc.x<=this.x1() )||( rc.x1()>=this.x && rc.x1()<=this.x1()  )
				) && (
					( rc.y>=this.y && rc.y<=this.y1() )||( rc.y1()>=this.y && rc.y1()<=this.y1() )
				)
			)||(
				(
					( this.x>=rc.x && this.x<=rc.x1() )||( this.x1()>=rc.x && this.x1()<=rc.x1()  )
				) && (
					( this.y>=rc.y && this.y<=rc.y1() )||( this.y1()>=rc.y && this.y1()<=rc.y1() )
				)
			)
		)||(
			(
				(
					( rc.x>=this.x && rc.x<=this.x1() )||( rc.x1()>=this.x && rc.x1()<=this.x1()  )
				) && (
					( this.y>=rc.y && this.y<=rc.y1() )||( this.y1()>=rc.y && this.y1()<=rc.y1() )
				)
			)||(
				(
					( this.x>=rc.x && this.x<=rc.x1() )||( this.x1()>=rc.x && this.x1()<=rc.x1()  )
				) && (
					( rc.y>=this.y && rc.y<=this.y1() )||( rc.y1()>=this.y && rc.y1()<=this.y1() )
				)
			)
		);
    },
  };
};
xdRectPacker.prototype = {
	width:0,
	height:0,
	side:'width',
	_x:'x',
	_y:'y',
	pack:[],
	findPlace:function( rc ){
		if( this.pack.length ){
			var i = 0;
			while( i<this.pack.length ){
				if( rc.intersect( this.pack[i] ) ){
					if( 1+rc[this._w]+this.pack[i][this._x+'1']()<this[this.side] ){
						rc[this._x] = this.pack[i][this._x+'1']()+1;
						i  = -1;
					}else{
						rc[this._y]+=1;
						rc[this._x]=0;
						i  = -1;
					}
				}
				i++;
			}
		}else{ rc.x = 0; rc.y = 0;};
		return rc;
	},
	fit:function (rcs){
		this.pack = [];
		for(var i=0;i<rcs.length;i++){
			this.pack.push( this.findPlace(rcs[i]) );
		}
	}
};
/**
	jQuery Plugin xdCloudTags
	Author:
		Valeriy Chupurnov <leroy@xdan.ru>, <http://xdan.ru>
	License:
		LGPL - Lesser General Public License
 */
var uid = 1;
$.fn.xdCloudTags  = function( options ){
	var settings = {
		'invert'    		: false,	// invert the rotation
		'rotate'			: true,		// use random rotate
		'sort'				: true,		// use sort to average size ( work when 'save'=false or ('save'=true && 'newsave'=true))
		'save'				: false,	// save one position
		'newsave'			: false,	// generate new save ( only for developers, use with 'save' for generate new rotate arrangement )
		'horizontal'		: false,	// orient cloud tags from left to right(default from top to bottom)
    };
	options = $.extend( settings, options );
	var slock = function (r,l,t,u){
		return {
			'rotate':r,
			'left':l,
			'top':t,
		};
	};
	var avgSort = function ( mass, name, name2 ){
		var srt = function( a,b ){
			return 
				parseInt(a[name])* parseInt(a[name2]) > parseInt(b[name])* parseInt(b[name2]);
		};
		var b=mass.slice( 0,mass.length/2 ).sort(srt);
		var c=mass.slice( mass.length/2 ).sort(srt).reverse();
		return b.concat(c);
	};
	return this.each(function(){
		var $cloud = $(this);
		$cloud.css('position','relative');
		var cloudUID = ( $cloud.attr('id') !== undefined )?$cloud.attr('id'):'xdCloud-'+uid++;
		var packer = new xdRectPacker( $cloud[options.horizontal?'height':'width'](),options.horizontal );
		var $tags = $cloud.children();
		var blocks = [];
		
		var memBlocks = {saved:false};
		if( options.save && localStorage.getItem( 'xdBlocks'+cloudUID ) && !options.newsave )
			memBlocks = JSON.parse(localStorage.getItem( 'xdBlocks'+cloudUID ));
		
		var i = 1;
		$tags.each(function(){
			var $this = $(this);
			var tagUID = cloudUID+'-'+i++;
			$this.attr('data-id',tagUID);
			$this.css({'position':'absolute',left:0,top:0});
			var w = parseInt($this.width()), h = parseInt($this.height());
			if( options.rotate ){
				if( (!memBlocks[tagUID] && Math.random()>0.5)||(memBlocks[tagUID] && memBlocks[tagUID].rotate) ){
					$this.addClass( 'vertical'+(options.invert?'-invert':'') );
					var b = w;
					w = h;
					h = b;
					$this.css({
						'marginTop':parseInt(h/2)-parseInt(w/2),
						'marginBottom':parseInt(h/2)-parseInt(w/2),
						'marginLeft':parseInt(w/2)-parseInt(h/2),
					});
				}
			}
			var elm = new xdRect(0,0,w,h);
			elm.elm = this;
			blocks.push(elm);
		});
		if( !options.save || !memBlocks.saved ){
			if( options.sort )
				blocks = avgSort( blocks,'w','h' );
			var coords = {};
			packer.fit( blocks );
			$(packer.pack).each(function(){
				var $this = $(this.elm);
				$this.css({'left':this.x,top:this.y});
				memBlocks[ $this.attr('data-id') ] = slock($this.hasClass('vertical')||$this.hasClass('vertical-invert'),this.x,this.y);
			});
		}else{
			$tags.each(function(){
				var tagUID = $(this).attr('data-id');
				if( memBlocks[tagUID] )
					$( this ).css({'left':memBlocks[tagUID].left,'top':memBlocks[tagUID].top});
			});
		}
		if( options.save && (!memBlocks['saved'] || !options.newsave) ){
			memBlocks['saved'] = true;
			localStorage.setItem( 'xdBlocks'+cloudUID,JSON.stringify(memBlocks) );
		}
		
	});
}
})(jQuery);