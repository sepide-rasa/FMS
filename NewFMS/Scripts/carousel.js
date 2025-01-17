

Ext.define('Ext.ux.Carousel', {
    extend: 'Ext.util.Observable',
    interval: 3,
    transitionDuration: 1,
    transitionType: 'carousel',
    transitionEasing: 'easeOut',
    itemSelector: 'img',
    activeSlide: 0,
    autoPlay: false,
    showPlayButton: false,
    pauseOnNavigate: false,
    wrap: false,
    freezeOnHover: false,
    navigationOnHover: false,
    hideNavigation: false,
    width: null,
    height: null,

    constructor: function (elId, config) {
        config = config || {};
        Ext.apply(this, config);

        Ext.ux.Carousel.superclass.constructor.call(this, config);

        this.addEvents(
            'beforeprev',
            'prev',
            'beforenext',
            'next',
            'change',
            'play',
            'pause',
            'freeze',
            'unfreeze'
        );

        this.el = Ext.get(elId);
        this.slides = this.els = [];

        if (this.autoPlay || this.showPlayButton) {
            this.wrap = true;
        };

        if (this.autoPlay && typeof config.showPlayButton === 'undefined') {
            this.showPlayButton = true;
        }

        this.initMarkup();
        this.initEvents();

        if (this.carouselSize > 0) {
            this.refresh();
        }
    },

    initMarkup: function () {
        var dh = Ext.core.DomHelper;

        this.carouselSize = 0;
        var items = this.el.select("img");  //replace itemSelector with "img" tag
        this.els.container = dh.append(this.el, { cls: 'ux-carousel-container' }, true);
        this.els.slidesWrap = dh.append(this.els.container, { cls: 'ux-carousel-slides-wrap' }, true);

        this.els.navigation = dh.append(this.els.container, { cls: 'ux-carousel-nav' }, true).hide();
        this.els.caption = dh.append(this.els.navigation, { tag: 'h2', cls: 'ux-carousel-caption' }, true);
        this.els.navNext = dh.append(this.els.navigation, { tag: 'a', href: '#', cls: 'ux-carousel-nav-next' }, true);

        if (this.showPlayButton) {
            this.els.navPlay = dh.append(this.els.navigation, { tag: 'a', href: '#', cls: 'ux-carousel-nav-play' }, true)
        }
        this.els.navPrev = dh.append(this.els.navigation, { tag: 'a', href: '#', cls: 'ux-carousel-nav-prev' }, true);

        // set the dimensions of the container
        this.slideWidth = this.width || this.el.getWidth(true);
        this.slideHeight = this.height || this.el.getHeight(true);
        this.els.container.setStyle({
            width: this.slideWidth + 'px',
            height: this.slideHeight + 'px'
        });

        this.els.caption.setWidth((this.slideWidth - (this.els.navNext.getWidth() * 2) - (this.showPlayButton ? this.els.navPlay.getWidth() : 0) - 20) + 'px')

        items.appendTo(this.els.slidesWrap).each(function (item) {
            item = item.wrap({ cls: 'ux-carousel-slide' });
            this.slides.push(item);
            item.setWidth(this.slideWidth + 'px').setHeight(this.slideHeight + 'px');
        }, this);
        this.carouselSize = this.slides.length;
        if (this.navigationOnHover) {
            this.els.navigation.setStyle('top', (-1 * this.els.navigation.getHeight()) + 'px');
        }
        this.el.clip();
    },

    initEvents: function () {
        this.els.navPrev.on('click', function (ev) {
            ev.preventDefault();
            var target = ev.getTarget();
            target.blur();
            if (Ext.fly(target).hasCls('ux-carousel-nav-disabled')) return;
            this.prev();
        }, this);

        this.els.navNext.on('click', function (ev) {
            ev.preventDefault();
            var target = ev.getTarget();
            target.blur();
            if (Ext.fly(target).hasCls('ux-carousel-nav-disabled')) return;
            this.next();
        }, this);

        if (this.showPlayButton) {
            this.els.navPlay.on('click', function (ev) {
                ev.preventDefault();
                ev.getTarget().blur();
                if (this.playing) {
                    this.pause();
                }
                else {
                    this.play();
                }
            }, this);
        };

        if (this.freezeOnHover) {
            this.els.container.on('mouseenter', function () {
                if (this.playing) {
                    this.fireEvent('freeze', this.slides[this.activeSlide]);
                    Ext.TaskManager.stop(this.playTask);
                }
            }, this);
            this.els.container.on('mouseleave', function () {
                if (this.playing) {
                    this.fireEvent('unfreeze', this.slides[this.activeSlide]);
                    Ext.TaskManager.start(this.playTask);
                }
            }, this, { buffer: (this.interval / 2) * 1000 });
        };

        if (this.navigationOnHover) {
            this.els.container.on('mouseenter', function () {
                if (!this.navigationShown) {
                    this.navigationShown = true;
                    this.els.navigation.stopAnimation(false).shift({
                        y: this.els.container.getY(),
                        duration: this.transitionDuration
                    })
                }
            }, this);

            this.els.container.on('mouseleave', function () {
                if (this.navigationShown) {
                    this.navigationShown = false;
                    this.els.navigation.stopAnimation(false).shift({
                        y: this.els.navigation.getHeight() - this.els.container.getY(),
                        duration: this.transitionDuration
                    })
                }
            }, this);
        }

        if (this.interval && this.autoPlay) {
            this.play();
        };
    },

    prev: function () {
        if (this.fireEvent('beforeprev') === false) {
            return;
        }
        if (this.pauseOnNavigate) {
            this.pause();
        }
        this.setSlide(this.activeSlide - 1);

        this.fireEvent('prev', this.activeSlide);
        return this;
    },

    next: function () {
        if (this.fireEvent('beforenext') === false) {
            return;
        }
        if (this.pauseOnNavigate) {
            this.pause();
        }
        console.log(this.activeSlide);

        this.setSlide(this.activeSlide + 1);

        this.fireEvent('next', this.activeSlide);
        return this;
    },

    play: function () {
        if (!this.playing) {
            this.playTask = this.playTask || {
                run: function () {
                    this.playing = true;
                    this.setSlide(this.activeSlide + 1);
                },
                interval: this.interval * 1000,
                scope: this
            };

            this.playTaskBuffer = this.playTaskBuffer || new Ext.util.DelayedTask(function () {
                Ext.TaskManager.start(this.playTask);
            }, this);

            this.playTaskBuffer.delay(this.interval * 1000);
            this.playing = true;
            if (this.showPlayButton) {
                this.els.navPlay.addCls('ux-carousel-playing');
            }
            this.fireEvent('play');
        }
        return this;
    },

    pause: function () {
        if (this.playing) {
            Ext.TaskManager.stop(this.playTask);
            this.playTaskBuffer.cancel();
            this.playing = false;
            if (this.showPlayButton) {
                this.els.navPlay.removeCls('ux-carousel-playing');
            }
            this.fireEvent('pause');
        }
        return this;
    },

    clear: function () {
        this.els.slidesWrap.update('');
        this.slides = [];
        this.carouselSize = 0;
        this.pause();
        return this;
    },

    add: function (el, refresh) {
        var item = Ext.fly(el).appendTo(this.els.slidesWrap).wrap({ cls: 'ux-carousel-slide' });
        item.setWidth(this.slideWidth + 'px').setHeight(this.slideHeight + 'px');
        this.slides.push(item);
        if (refresh) {
            this.refresh();
        }
        return this;
    },

    refresh: function () {
        this.carouselSize = this.slides.length;
        this.els.slidesWrap.setWidth((this.slideWidth * this.carouselSize) + 'px');
        if (this.carouselSize > 0) {
            if (!this.hideNavigation) this.els.navigation.show();
            this.activeSlide = 0;
            this.setSlide(0, true);
        }
        return this;
    },

    setSlide: function (index, initial) {
        if (!this.wrap && !this.slides[index]) {
            return;
        }
        else if (this.wrap) {
            if (index < 0) {
                index = this.carouselSize - 1;
            }
            else if (index > this.carouselSize - 1) {
                index = 0;
            }
        }
        if (!this.slides[index]) {
            return;
        }

        this.els.caption.update(this.slides[index].child(':first-child', true).title || '');
        var offset = index * this.slideWidth;
        if (!initial) {
            switch (this.transitionType) {
                case 'fade':
                    this.slides[index].setOpacity(0);
                    this.slides[this.activeSlide].stopAnimation(false).fadeOut({
                        duration: this.transitionDuration / 2,
                        callback: function () {
                            this.els.slidesWrap.setStyle('left', (-1 * offset) + 'px');
                            this.slides[this.activeSlide].setOpacity(1);
                            this.slides[index].fadeIn({
                                duration: this.transitionDuration / 2
                            });
                        },
                        scope: this
                    })
                    break;

                default:
                    var xNew = (-1 * offset) + this.els.container.getX();
                    //TODO: check this
                    console.log(this.els);
                    this.els.slidesWrap.stopAnimation(false);
                    this.els.slidesWrap.shift({
                        duration: this.transitionDuration,
                        x: xNew,
                        easing: this.transitionEasing
                    });
                    console.log('dsets');
                    break;
            }
        }
        else {
            this.els.slidesWrap.setStyle('left', '0');
        }

        this.activeSlide = index;
        this.updateNav();
        this.fireEvent('change', this.slides[index], index);
    },

    updateNav: function () {
        this.els.navPrev.removeCls('ux-carousel-nav-disabled');
        this.els.navNext.removeCls('ux-carousel-nav-disabled');
        if (!this.wrap) {
            if (this.activeSlide === 0) {
                this.els.navPrev.addCls('ux-carousel-nav-disabled');
            }
            if (this.activeSlide === this.carouselSize - 1) {
                this.els.navNext.addCls('ux-carousel-nav-disabled');
            }
        }
    }

});