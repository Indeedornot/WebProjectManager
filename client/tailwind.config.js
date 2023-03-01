/** @type {import('tailwindcss').Config} */
module.exports = {
	content: ['./**/*.{razor,cshtml,html}'],
	theme: {
		extend: {
			backgroundColor: {
				dark: 'rgb(var(--color-bg-dark) / <alpha-value>)',
				default: 'rgb(var(--color-bg-default) / <alpha-value>)',
				hover: 'rgb(var(--color-bg-hover) / <alpha-value>)',
				subtle: 'rgb(var(--color-bg-subtle) / <alpha-value>)',
				icon: 'rgb(var(--color-bg-icon) / <alpha-value>)',
				emphasis: 'rgb(var(--color-bg-emphasis) / <alpha-value>)',
				overlay: 'rgb(var(--color-bg-overlay) / <alpha-value>)',
				accent: 'rgb(var(--color-bg-accent) / <alpha-value>)',
				online: 'rgb(var(--color-bg-online) / <alpha-value>)',
				offline: 'rgb(var(--color-bg-offline) / <alpha-value>)',
				busy: 'rgb(var(--color-bg-busy) / <alpha-value>)',
				away: 'rgb(var(--color-bg-away) / <alpha-value>)',
			},
		},
		screens: {
			sm: '0px',
			xs: '380px',
			md: '544px',
			lg: '768px',
		},
		textColor: {
			default: 'rgb(var(--color-fg-default) / <alpha-value>)',
			muted: 'rgb(var(--color-fg-muted) / <alpha-value>)',
			icon: 'rgb(var(--color-fg-icon) / <alpha-value>)',
			'icon-bg': 'rgb(var(--color-fg-icon-bg) / <alpha-value>)',
			emphasis: 'rgb(var(--color-fg-emphasis) / <alpha-value>)',
			accent: 'rgb(var(--color-fg-accent) / <alpha-value>)',
			anchor: 'rgb(var(--color-fg-anchor) / <alpha-value>)',
			disabled: 'rgb(var(--color-fg-disabled) / <alpha-value>)',
		},
		caretColor: {
			default: 'rgb(var(--color-caret-default) / <alpha-value>)',
		},
		borderColor: {
			default: 'rgb(var(--color-border-default) / <alpha-value>)',
			muted: 'rgb(var(--color-border-muted) / <alpha-value>)',
			subtle: 'rgb(var(--color-border-subtle))',
			emphasis: 'rgb(var(--color-border-emphasis))',
		},
		boxShadow: {
			default: 'var(--color-shadow-default)',
			medium: 'var(--color-shadow-medium)',
			large: 'var(--color-shadow-large)',
			'extra-large': 'var(--color-shadow-extra-large)',
			'outline-default': 'var(--color-shadow-outline-default)',
			'outline-muted': 'var(--color-shadow-outline-muted)',
			ambient: 'var(--color-shadow-ambient)',
		},
	},
	safelist: ['bg-online', 'bg-offline', 'bg-busy', 'bg-away'],
	plugins: [
		function ({addVariant}) {
			addVariant('child', '& > *');
			addVariant('child-hover', '& > *:hover');
		},
	],
};
