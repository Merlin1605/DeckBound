// Sprite-uri DeckBound: SVG desenat din cod, randat in PNG transparent cu Chromium.
const fs = require('fs');
const path = require('path');
const { chromium } = require('playwright');
const OUT = process.argv[2];

const grit = (id = 'grit', freq = 0.85, lo = 0.72) => `
<filter id="${id}" filterUnits="userSpaceOnUse" x="-50" y="-50" width="2100" height="1200" color-interpolation-filters="sRGB">
  <feTurbulence type="fractalNoise" baseFrequency="${freq}" numOctaves="3" seed="7" result="n"/>
  <feColorMatrix in="n" type="saturate" values="0" result="m"/>
  <feComponentTransfer in="m" result="t">
    <feFuncR type="linear" slope="${(1 - lo) * 2}" intercept="${lo - (1 - lo) / 2}"/>
    <feFuncG type="linear" slope="${(1 - lo) * 2}" intercept="${lo - (1 - lo) / 2}"/>
    <feFuncB type="linear" slope="${(1 - lo) * 2}" intercept="${lo - (1 - lo) / 2}"/>
    <feFuncA type="linear" slope="0" intercept="1"/>
  </feComponentTransfer>
  <feBlend in="SourceGraphic" in2="t" mode="multiply" result="b"/>
  <feComposite in="b" in2="SourceGraphic" operator="in"/>
</filter>`;
const glow = (id, std) => `<filter id="${id}" x="-50%" y="-50%" width="200%" height="200%"><feGaussianBlur stdDeviation="${std}"/></filter>`;
const svg = (w, h, body, defs = '') =>
  `<svg xmlns="http://www.w3.org/2000/svg" width="${w}" height="${h}" viewBox="0 0 ${w} ${h}"><defs>${grit()}${defs}</defs>${body}</svg>`;
// Membru gros cu contur negru: intai conturul, apoi culoarea.
const limb = (d, w, fill) => `<path d="${d}" fill="none" stroke="#0a0706" stroke-width="${w + 8}" stroke-linecap="round" stroke-linejoin="round"/><path d="${d}" fill="none" stroke="${fill}" stroke-width="${w}" stroke-linecap="round" stroke-linejoin="round"/>`;
const OL = 'stroke="#0a0706" stroke-width="4" stroke-linejoin="round"';

const sprites = {};

// ---------- PERSONAJE ----------
sprites['Characters/hero_bruiser'] = svg(512, 512, `
<ellipse cx="256" cy="492" rx="160" ry="16" fill="#000" opacity="0.45"/>
<g filter="url(#grit)">
  <!-- picioare -->
  <path d="M196 330 L252 330 L246 462 L198 462 Z" fill="#3a2c22" ${OL}/>
  <path d="M262 330 L322 330 L338 462 L286 462 Z" fill="#33271e" ${OL}/>
  <path d="M188 452 L252 452 L254 490 L180 490 Q178 466 188 452Z" fill="#1c1410" ${OL}/>
  <path d="M282 452 L342 452 Q356 468 352 490 L280 490 Z" fill="#1c1410" ${OL}/>
  <path d="M200 400 L248 396 M268 396 L330 402" stroke="#1a120d" stroke-width="5"/>
  <!-- brat stang (in spate) -->
  ${limb('M168 205 Q132 260 138 330', 50, '#8f6450')}
  <path d="M112 318 h54 v40 q0 14 -14 14 h-26 q-14 0 -14 -14Z" fill="#6e5a48" ${OL}/>
  <path d="M114 332 h50 M114 346 h50" stroke="#3d3026" stroke-width="4"/>
  <!-- tors -->
  <path d="M150 205 Q156 160 208 150 L306 150 Q356 160 364 205 L338 340 L180 340 Z" fill="#a0735a" ${OL}/>
  <path d="M256 175 L256 300 M200 240 Q228 262 256 248 Q284 262 314 240" stroke="#6b4636" stroke-width="5" fill="none"/>
  <path d="M206 290 Q230 300 254 292 M262 292 Q286 300 308 290" stroke="#6b4636" stroke-width="4" fill="none"/>
  <!-- curea diagonala -->
  <path d="M160 190 L196 180 L340 330 L304 340 Z" fill="#4a3324" ${OL}/>
  <circle cx="232" cy="238" r="7" fill="#9c8a6a" stroke="#0a0706" stroke-width="3"/>
  <circle cx="276" cy="284" r="7" fill="#9c8a6a" stroke="#0a0706" stroke-width="3"/>
  <!-- centura -->
  <path d="M176 318 L342 318 L338 350 L178 350 Z" fill="#2c1e15" ${OL}/>
  <rect x="240" y="314" width="36" height="40" rx="4" fill="#8a7650" ${OL}/>
  <rect x="249" y="323" width="18" height="22" fill="#2c1e15"/>
  <!-- blana pe umar -->
  <path d="M132 214 Q128 160 182 146 Q222 140 236 162 Q214 176 200 172 Q206 196 182 196 Q178 222 152 226 Z" fill="#5a4a3a" ${OL}/>
  <path d="M150 200 l10 -14 M170 180 l12 -12 M196 160 l10 -8 M160 214 l14 -10" stroke="#2e241b" stroke-width="4"/>
  <!-- gat si cap -->
  <path d="M232 128 L290 128 L296 160 L226 160 Z" fill="#8f6450" ${OL}/>
  <ellipse cx="262" cy="98" rx="44" ry="50" fill="#a87b62" ${OL}/>
  <ellipse cx="220" cy="102" rx="9" ry="14" fill="#8f6450" ${OL}/>
  <path d="M220 104 Q224 150 262 158 Q302 150 306 104 Q296 126 262 126 Q234 126 220 104Z" fill="#3b2a1e" ${OL}/>
  <path d="M244 132 Q262 124 282 132" stroke="#1a120d" stroke-width="4" fill="none"/>
  <path d="M240 86 L262 92 M276 92 L300 84" stroke="#1a120d" stroke-width="7" stroke-linecap="round"/>
  <path d="M248 98 h12 M280 98 h12" stroke="#e8d8b0" stroke-width="4" stroke-linecap="round"/>
  <path d="M286 64 L300 112" stroke="#7a2a22" stroke-width="5" stroke-linecap="round"/>
  <path d="M232 62 Q262 44 296 60" stroke="#c49a7e" stroke-width="5" fill="none" opacity="0.6"/>
  <!-- brat drept (garda in fata) -->
  ${limb('M346 200 Q392 238 372 286', 52, '#a0735a')}
  ${limb('M372 286 Q360 300 338 290', 44, '#a0735a')}
  <path d="M300 262 h56 v44 q0 14 -14 14 h-28 q-14 0 -14 -14Z" fill="#7a6450" ${OL}/>
  <path d="M302 276 h52 M302 292 h52" stroke="#3d3026" stroke-width="4"/>
  <path d="M308 262 l6 -12 l6 12 M324 262 l6 -12 l6 12 M340 262 l6 -12 l6 12" fill="#b8b0a0" stroke="#0a0706" stroke-width="3"/>
</g>`);

sprites['Characters/enemy_cultist'] = svg(512, 512, `
<ellipse cx="262" cy="492" rx="150" ry="16" fill="#000" opacity="0.45"/>
<g filter="url(#grit)">
  <!-- roba -->
  <path d="M206 170 Q170 300 136 486 L160 470 L180 492 L206 470 L232 494 L258 472 L284 494 L310 470 L336 492 L360 470 L388 486 Q350 300 318 170 Z" fill="#3a1a1a" ${OL}/>
  <path d="M232 220 Q214 360 198 470 M262 214 L262 476 M292 220 Q312 360 330 470" stroke="#1e0c0c" stroke-width="5" fill="none"/>
  <path d="M206 300 Q262 318 324 300" stroke="#7a6a4a" stroke-width="9" fill="none" stroke-linecap="round"/>
  <path d="M232 306 l-6 60 M240 308 l2 52" stroke="#7a6a4a" stroke-width="6" stroke-linecap="round"/>
  <!-- maneca si mana -->
  <path d="M220 190 Q170 220 150 268 L186 290 Q206 250 240 236 Z" fill="#331616" ${OL}/>
  <path d="M146 262 q-14 4 -16 20 q8 14 26 10 q14 -6 10 -22 Z" fill="#b8b49a" ${OL}/>
  <!-- seceră -->
  <path d="M150 300 L118 120" stroke="#0a0706" stroke-width="14" stroke-linecap="round"/>
  <path d="M150 300 L118 120" stroke="#4a3626" stroke-width="8" stroke-linecap="round"/>
  <path d="M118 124 Q40 112 30 190 Q62 140 120 150 Z" fill="#6a6a62" ${OL}/>
  <path d="M42 170 Q70 140 112 140" stroke="#a8a89a" stroke-width="3" fill="none"/>
  <path d="M60 160 l6 10 M80 150 l4 10" stroke="#5a1a14" stroke-width="4"/>
  <!-- gluga -->
  <path d="M196 196 Q186 92 262 66 Q338 92 330 196 Q300 176 262 176 Q226 176 196 196Z" fill="#451f1f" ${OL}/>
  <path d="M220 178 Q222 112 262 100 Q302 112 306 178 Q284 168 262 168 Q240 168 220 178Z" fill="#070404"/>
  <path d="M262 70 Q250 110 254 150" stroke="#2a1010" stroke-width="4" fill="none"/>
</g>
<g>
  <ellipse cx="246" cy="138" rx="9" ry="5" fill="#e0ff6a" filter="url(#eyeGlow)"/>
  <ellipse cx="282" cy="138" rx="9" ry="5" fill="#e0ff6a" filter="url(#eyeGlow)"/>
  <ellipse cx="246" cy="138" rx="5" ry="3" fill="#f8ffd0"/>
  <ellipse cx="282" cy="138" rx="5" ry="3" fill="#f8ffd0"/>
</g>`, glow('eyeGlow', 4));

// ---------- CARTI ----------
const frame = (accent, accentDark, label) => svg(400, 560, `
<g mask="url(#win)"><g filter="url(#grit)">
  <rect x="4" y="4" width="392" height="552" rx="22" fill="url(#metal)" stroke="#0a0706" stroke-width="6"/>
  <rect x="16" y="16" width="368" height="528" rx="14" fill="none" stroke="#8a6d3e" stroke-width="3"/>
  <rect x="40" y="70" width="320" height="214" fill="none" stroke="#0a0706" stroke-width="6"/>
  <rect x="34" y="64" width="332" height="226" fill="none" stroke="#8a6d3e" stroke-width="4"/>
  <rect x="46" y="350" width="308" height="168" rx="6" fill="#2b241d" stroke="#0a0706" stroke-width="4"/>
  <rect x="54" y="358" width="292" height="152" rx="4" fill="none" stroke="#4a3d2e" stroke-width="2"/>
  <path d="M14 296 L386 296 L372 318 L386 340 L14 340 L28 318 Z" fill="${accent}" stroke="#0a0706" stroke-width="5"/>
  <path d="M32 302 L368 302" stroke="${accentDark}" stroke-width="3" opacity="0.7"/>
  <path d="M150 530 L250 530 L240 552 L160 552 Z" fill="${accent}" stroke="#0a0706" stroke-width="4"/>
  <circle cx="48" cy="48" r="38" fill="#1a1410" stroke="#0a0706" stroke-width="5"/>
  <circle cx="48" cy="48" r="32" fill="url(#orb)" stroke="#8a6d3e" stroke-width="3"/>
  <path d="M352 20 l26 0 l0 26 M48 540 l-26 0 l0 -26 M378 514 l0 26 l-26 0" stroke="#8a6d3e" stroke-width="4" fill="none"/>
</g></g>
<text x="200" y="548" font-family="serif" font-size="13" fill="#e8d8b0" text-anchor="middle" opacity="0">${label}</text>`,
`<mask id="win" maskUnits="userSpaceOnUse" x="0" y="0" width="400" height="560"><rect width="400" height="560" fill="#fff"/><rect x="42" y="72" width="316" height="210" fill="#000"/></mask>
 <linearGradient id="metal" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#4a4036"/><stop offset="0.5" stop-color="#2e2620"/><stop offset="1" stop-color="#1c1713"/></linearGradient>
 <radialGradient id="orb" cx="0.4" cy="0.35" r="0.7"><stop offset="0" stop-color="#ffd27a"/><stop offset="0.5" stop-color="#d97a1e"/><stop offset="1" stop-color="#5a2208"/></radialGradient>`);
sprites['Cards/card_frame_attack'] = frame('#7a1c16', '#c0473a', 'ATAC');
sprites['Cards/card_frame_skill'] = frame('#2f4a5a', '#6a8ea0', 'ABILITATE');

sprites['Cards/card_back'] = svg(400, 560, `
<g filter="url(#grit)">
  <rect x="4" y="4" width="392" height="552" rx="22" fill="#2a1612" stroke="#0a0706" stroke-width="6"/>
  <rect x="18" y="18" width="364" height="524" rx="14" fill="none" stroke="#8a6d3e" stroke-width="4"/>
  <rect x="34" y="34" width="332" height="492" rx="8" fill="none" stroke="#5a3d22" stroke-width="2"/>
  <circle cx="200" cy="280" r="110" fill="#1a0c0a" stroke="#8a6d3e" stroke-width="5"/>
  <circle cx="200" cy="280" r="92" fill="none" stroke="#5a3d22" stroke-width="3"/>
  <path d="M200 190 L224 256 L290 280 L224 304 L200 370 L176 304 L110 280 L176 256 Z" fill="#7a1c16" stroke="#0a0706" stroke-width="4"/>
  <circle cx="200" cy="280" r="18" fill="#c0473a" stroke="#0a0706" stroke-width="4"/>
  <path d="M60 60 l40 0 M60 60 l0 40 M340 60 l-40 0 M340 60 l0 40 M60 500 l40 0 M60 500 l0 -40 M340 500 l-40 0 M340 500 l0 -40" stroke="#8a6d3e" stroke-width="5"/>
</g>`);

// Arta 480x320 (incape in fereastra 320x214 a ramei, aspect 3:2)
const fistShape = (x, y, s, skin, wrap) => `<g transform="translate(${x} ${y}) scale(${s})">
  <path d="M-150 30 L-20 -10 L-10 70 L-150 90 Z" fill="${wrap}" ${OL}/>
  <path d="M-140 40 L-20 4 M-140 62 L-16 30 M-140 82 L-14 56" stroke="#3d3026" stroke-width="4"/>
  <rect x="-30" y="-40" width="120" height="120" rx="26" fill="${skin}" ${OL}/>
  <path d="M60 -40 q30 0 30 26 v8 q0 26 -30 26 M60 -6 q30 0 30 24 v6 q0 24 -30 24 M60 26 q30 0 30 24 v4 q0 26 -30 26" fill="${skin}" ${OL}/>
  <path d="M-10 -40 q0 -20 30 -20 h40 q30 0 30 20" fill="${skin}" ${OL}/>
  <path d="M10 10 q20 -16 46 -6" stroke="#5a3a2a" stroke-width="4" fill="none"/>
  <path d="M-30 40 q-10 30 30 40" fill="${skin}" ${OL}/>
</g>`;
sprites['Cards/art_strike'] = svg(480, 320, `
<rect width="480" height="320" fill="url(#bgS)"/>
<g opacity="0.85">${[0,1,2,3,4,5,6,7,8,9,10,11].map(i => {const a=i*Math.PI/6; return `<path d="M350 160 L${350+Math.cos(a)*200} ${160+Math.sin(a)*200} L${350+Math.cos(a+0.12)*200} ${160+Math.sin(a+0.12)*200}Z" fill="#e8a040" opacity="0.25"/>`}).join('')}</g>
<circle cx="350" cy="160" r="70" fill="#ffcf7a" opacity="0.35" filter="url(#soft)"/>
<path d="M40 110 h120 M20 160 h130 M50 210 h110" stroke="#c8a070" stroke-width="6" stroke-linecap="round" opacity="0.5"/>
<g filter="url(#grit)">${fistShape(250, 150, 1.05, '#a0735a', '#6e5a48')}</g>
<path d="M378 92 l18 -22 M400 140 l28 -6 M400 200 l26 12 M372 240 l14 22" stroke="#fff0c0" stroke-width="7" stroke-linecap="round"/>
<rect width="480" height="320" fill="url(#vig)"/>`,
`<radialGradient id="bgS" cx="0.7" cy="0.5" r="0.8"><stop offset="0" stop-color="#7a3a1a"/><stop offset="1" stop-color="#1a0c08"/></radialGradient>
 <radialGradient id="vig" cx="0.5" cy="0.5" r="0.75"><stop offset="0.6" stop-color="#000" stop-opacity="0"/><stop offset="1" stop-color="#000" stop-opacity="0.75"/></radialGradient>
 ${glow('soft', 18)}`);

sprites['Cards/art_defend'] = svg(480, 320, `
<rect width="480" height="320" fill="url(#bgD)"/>
<g filter="url(#grit)">
  <path d="M240 34 L346 66 Q352 196 240 292 Q128 196 134 66 Z" fill="#5a3e28" stroke="#0a0706" stroke-width="8"/>
  <path d="M190 52 L192 250 M240 38 L240 288 M290 52 L288 250" stroke="#2e2014" stroke-width="5"/>
  <path d="M240 34 L346 66 Q352 196 240 292 Q128 196 134 66 Z" fill="none" stroke="#8a8a82" stroke-width="16"/>
  <path d="M240 34 L346 66 Q352 196 240 292 Q128 196 134 66 Z" fill="none" stroke="#0a0706" stroke-width="3"/>
  <path d="M240 54 L328 80 Q330 190 240 270 Q150 190 152 80 Z" fill="none" stroke="#0a0706" stroke-width="3"/>
  <circle cx="240" cy="150" r="34" fill="#7a7a72" stroke="#0a0706" stroke-width="5"/>
  <circle cx="232" cy="142" r="10" fill="#b8b8ae" opacity="0.6"/>
  ${[[150,80],[330,80],[160,170],[320,170],[240,46],[240,268]].map(([x,y])=>`<circle cx="${x}" cy="${y}" r="6" fill="#c8c8be" stroke="#0a0706" stroke-width="2"/>`).join('')}
  <path d="M286 110 l14 18 l-6 10 M196 210 l-12 14" stroke="#1a120d" stroke-width="4" fill="none"/>
</g>
<path d="M96 70 Q70 160 108 250 M384 70 Q410 160 372 250" stroke="#a0c8d8" stroke-width="5" fill="none" opacity="0.35"/>
<rect width="480" height="320" fill="url(#vig)"/>`,
`<radialGradient id="bgD" cx="0.5" cy="0.45" r="0.8"><stop offset="0" stop-color="#3a5262"/><stop offset="1" stop-color="#0a1014"/></radialGradient>
 <radialGradient id="vig" cx="0.5" cy="0.5" r="0.75"><stop offset="0.6" stop-color="#000" stop-opacity="0"/><stop offset="1" stop-color="#000" stop-opacity="0.75"/></radialGradient>`);

const drop = (x, y, s) => `<path d="M${x} ${y} q${-10*s} ${16*s} 0 ${24*s} q${10*s} ${-8*s} 0 ${-24*s}Z" fill="#a01818" stroke="#2a0404" stroke-width="2"/>`;
sprites['Cards/art_bloodstrike'] = svg(480, 320, `
<rect width="480" height="320" fill="url(#bgB)"/>
<path d="M330 120 c-16 -24 -54 -12 -40 22 c8 18 40 38 40 38 c0 0 32 -20 40 -38 c14 -34 -24 -46 -40 -22Z" fill="#ff4040" opacity="0.6" filter="url(#soft)"/>
<path d="M330 120 c-16 -24 -54 -12 -40 22 c8 18 40 38 40 38 c0 0 32 -20 40 -38 c14 -34 -24 -46 -40 -22Z" fill="#b01c1c" stroke="#2a0404" stroke-width="4"/>
<g filter="url(#grit)">
  <path d="M60 280 L340 40 L360 56 L84 296 Z" fill="#d0d0c8" stroke="#0a0706" stroke-width="5"/>
  <path d="M70 284 L344 48" stroke="#ffffff" stroke-width="3" opacity="0.6"/>
  <path d="M120 230 L210 154 L230 176 L140 250 Z" fill="#8a1010" opacity="0.85"/>
  <path d="M40 300 L64 276 L84 296 L60 320Z" fill="#3a2a1e" stroke="#0a0706" stroke-width="4"/>
  <path d="M30 270 L90 330" stroke="#5a4a3a" stroke-width="12" stroke-linecap="round"/>
</g>
<path d="M90 120 Q200 60 300 110" stroke="#c01818" stroke-width="10" fill="none" stroke-linecap="round" opacity="0.8"/>
<path d="M300 110 l-12 -14 M300 110 l-18 4" stroke="#c01818" stroke-width="8" stroke-linecap="round"/>
${drop(170,190,1.2)}${drop(200,230,1)}${drop(150,250,0.9)}${drop(250,200,0.8)}${drop(120,140,0.8)}
<rect width="480" height="320" fill="url(#vig)"/>`,
`<radialGradient id="bgB" cx="0.6" cy="0.4" r="0.8"><stop offset="0" stop-color="#4a1010"/><stop offset="1" stop-color="#0c0404"/></radialGradient>
 <radialGradient id="vig" cx="0.5" cy="0.5" r="0.75"><stop offset="0.6" stop-color="#000" stop-opacity="0"/><stop offset="1" stop-color="#000" stop-opacity="0.75"/></radialGradient>
 ${glow('soft', 14)}`);

// ---------- ICONITE 128 ----------
sprites['Icons/icon_energy'] = svg(128, 128, `
<circle cx="64" cy="64" r="58" fill="#ff9a2a" opacity="0.4" filter="url(#g)"/>
<circle cx="64" cy="64" r="50" fill="url(#o)" stroke="#0a0706" stroke-width="6"/>
<path d="M64 26 Q86 54 76 74 Q86 70 84 60 Q98 86 78 100 Q64 108 50 100 Q30 86 44 60 Q44 72 52 74 Q44 50 64 26Z" fill="#ffe8a0" stroke="#7a3008" stroke-width="3"/>
<path d="M64 60 Q74 78 66 94 Q56 96 54 84 Q56 74 64 60Z" fill="#fff8e0"/>`,
`<radialGradient id="o" cx="0.4" cy="0.35" r="0.7"><stop offset="0" stop-color="#ffb84a"/><stop offset="0.6" stop-color="#c0500c"/><stop offset="1" stop-color="#4a1804"/></radialGradient>${glow('g', 6)}`);

sprites['Icons/icon_armor'] = svg(128, 128, `
<path d="M64 8 L112 24 Q114 84 64 122 Q14 84 16 24 Z" fill="url(#s)" stroke="#0a0706" stroke-width="7" stroke-linejoin="round"/>
<path d="M64 20 L100 32 Q100 80 64 108 Q28 80 28 32 Z" fill="none" stroke="#e0e0d8" stroke-width="3" opacity="0.5"/>
<path d="M64 20 L64 108" stroke="#3a3a36" stroke-width="4"/>`,
`<linearGradient id="s" x1="0" y1="0" x2="1" y2="1"><stop offset="0" stop-color="#c8c8c0"/><stop offset="0.5" stop-color="#7a7a74"/><stop offset="1" stop-color="#3a3a36"/></linearGradient>`);

sprites['Icons/icon_hp'] = svg(128, 128, `
<path d="M64 112 C30 88 8 66 12 40 C16 14 50 8 64 34 C78 8 112 14 116 40 C120 66 98 88 64 112Z" fill="url(#h)" stroke="#0a0706" stroke-width="7" stroke-linejoin="round"/>
<path d="M32 36 Q36 24 50 26" stroke="#ffb0a0" stroke-width="6" fill="none" stroke-linecap="round" opacity="0.7"/>`,
`<radialGradient id="h" cx="0.4" cy="0.35" r="0.7"><stop offset="0" stop-color="#e04040"/><stop offset="0.7" stop-color="#8a1010"/><stop offset="1" stop-color="#3a0404"/></radialGradient>`);

sprites['Icons/icon_intent_attack'] = svg(128, 128, `
<g transform="rotate(45 64 64)">
  <path d="M58 6 L70 6 L72 84 L56 84 Z" fill="url(#b)" stroke="#0a0706" stroke-width="5" stroke-linejoin="round"/>
  <path d="M64 0 L72 8 L56 8 Z" fill="#d8d8d0" stroke="#0a0706" stroke-width="4" stroke-linejoin="round"/>
  <rect x="38" y="84" width="52" height="12" rx="4" fill="#8a6d3e" stroke="#0a0706" stroke-width="5"/>
  <rect x="57" y="96" width="14" height="22" fill="#3a2a1e" stroke="#0a0706" stroke-width="5"/>
  <circle cx="64" cy="122" r="7" fill="#8a6d3e" stroke="#0a0706" stroke-width="4"/>
</g>
<path d="M30 104 l-10 10 M20 92 l-10 4 M40 116 l-4 10" stroke="#c01818" stroke-width="6" stroke-linecap="round"/>`,
`<linearGradient id="b" x1="0" y1="0" x2="1" y2="0"><stop offset="0" stop-color="#e8e8e0"/><stop offset="1" stop-color="#8a8a82"/></linearGradient>`);

sprites['Icons/icon_draw_pile'] = svg(128, 128, `
<rect x="34" y="14" width="64" height="92" rx="8" fill="#3a1e18" stroke="#0a0706" stroke-width="5" transform="rotate(10 66 60)"/>
<rect x="28" y="20" width="64" height="92" rx="8" fill="#2a1612" stroke="#0a0706" stroke-width="5"/>
<rect x="36" y="28" width="48" height="76" rx="4" fill="none" stroke="#8a6d3e" stroke-width="3"/>
<path d="M60 46 L70 66 L60 86 L50 66 Z" fill="#7a1c16" stroke="#0a0706" stroke-width="3"/>`);

sprites['Icons/icon_discard_pile'] = svg(128, 128, `
<rect x="34" y="20" width="64" height="92" rx="8" fill="#2a2420" stroke="#0a0706" stroke-width="5" transform="rotate(-12 66 66)"/>
<rect x="30" y="20" width="64" height="92" rx="8" fill="#3a322c" stroke="#0a0706" stroke-width="5" transform="rotate(6 62 66)"/>
<path d="M44 56 L84 96 M84 56 L44 96" stroke="#7a2a22" stroke-width="8" stroke-linecap="round"/>`);

// ---------- UI ----------
sprites['UI/bar_frame'] = svg(320, 40, `
<g filter="url(#grit)"><rect x="2" y="2" width="316" height="36" rx="8" fill="#120e0c" stroke="#0a0706" stroke-width="4"/>
<rect x="5" y="5" width="310" height="30" rx="6" fill="none" stroke="#6a5432" stroke-width="3"/></g>`);
sprites['UI/bar_fill'] = svg(304, 24, `<rect width="304" height="24" rx="5" fill="url(#f)"/>`,
`<linearGradient id="f" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#ffffff"/><stop offset="0.5" stop-color="#d8d8d8"/><stop offset="1" stop-color="#9a9a9a"/></linearGradient>`);
sprites['UI/button_end_turn'] = svg(280, 90, `
<g filter="url(#grit)">
<path d="M14 6 L266 6 L276 45 L266 84 L14 84 L4 45 Z" fill="url(#bt)" stroke="#0a0706" stroke-width="6" stroke-linejoin="round"/>
<path d="M24 16 L256 16 L264 45 L256 74 L24 74 L16 45 Z" fill="none" stroke="#c8a060" stroke-width="3"/>
</g>`,
`<linearGradient id="bt" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#8a2a1e"/><stop offset="1" stop-color="#3a0e0a"/></linearGradient>`);
sprites['UI/intent_bubble'] = svg(160, 80, `
<rect x="4" y="4" width="152" height="72" rx="14" fill="#120e0c" opacity="0.85" stroke="#6a5432" stroke-width="3"/>`);

// ---------- FUNDAL ----------
const bricks = [];
for (let r = 0; r < 14; r++) for (let c = 0; c < 22; c++) {
  const x = c * 92 - (r % 2) * 46, y = r * 46;
  bricks.push(`<rect x="${x}" y="${y}" width="88" height="42" rx="3" fill="#${['221c18','1e1916','26201b','1b1714'][(r*7+c*3)%4]}"/>`);
}
const torch = x => `
<circle cx="${x}" cy="360" r="260" fill="url(#tg)"/>
<path d="M${x-14} 400 L${x+14} 400 L${x+8} 470 L${x-8} 470 Z" fill="#2a1e14" stroke="#0a0706" stroke-width="4"/>
<path d="M${x} 320 Q${x+26} 364 ${x+12} 398 L${x-12} 398 Q${x-26} 364 ${x} 320Z" fill="#ffb040" filter="url(#fl)"/>
<path d="M${x} 344 Q${x+12} 372 ${x+6} 396 L${x-6} 396 Q${x-12} 372 ${x} 344Z" fill="#fff0b0"/>`;
sprites['Backgrounds/bg_crypt'] = svg(1920, 1080, `
<rect width="1920" height="1080" fill="#0c0a09"/>
<g filter="url(#grit)">${bricks.join('')}</g>
${[360, 960, 1560].map(x => `<path d="M${x-170} 700 L${x-170} 330 Q${x-170} 160 ${x} 150 Q${x+170} 160 ${x+170} 330 L${x+170} 700 Z" fill="#070505" stroke="#2e2620" stroke-width="18"/>`).join('')}
${torch(660)}${torch(1260)}
<path d="M0 720 L1920 720 L1920 1080 L0 1080 Z" fill="url(#floor)"/>
<g opacity="0.5" stroke="#0a0806" stroke-width="4">${[0,1,2,3].map(i=>`<path d="M0 ${760+i*90} L1920 ${760+i*90}"/>`).join('')}${Array.from({length:13},(_,i)=>`<path d="M${960+(i-6)*160} 720 L${960+(i-6)*420} 1080"/>`).join('')}</g>
<rect y="640" width="1920" height="200" fill="url(#fog)"/>
<rect width="1920" height="1080" fill="url(#vg)"/>`,
`<radialGradient id="tg"><stop offset="0" stop-color="#ff9a3a" stop-opacity="0.35"/><stop offset="1" stop-color="#ff9a3a" stop-opacity="0"/></radialGradient>
 <linearGradient id="floor" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#1c1714"/><stop offset="1" stop-color="#0a0807"/></linearGradient>
 <linearGradient id="fog" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#5a5048" stop-opacity="0"/><stop offset="0.5" stop-color="#5a5048" stop-opacity="0.18"/><stop offset="1" stop-color="#5a5048" stop-opacity="0"/></linearGradient>
 <radialGradient id="vg" cx="0.5" cy="0.5" r="0.75"><stop offset="0.5" stop-color="#000" stop-opacity="0"/><stop offset="1" stop-color="#000" stop-opacity="0.85"/></radialGradient>
 ${glow('fl', 3)}`);

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  for (const [name, s] of Object.entries(sprites)) {
    const w = +s.match(/width="(\d+)"/)[1], h = +s.match(/height="(\d+)"/)[1];
    const file = path.join(OUT, name + '.png');
    fs.mkdirSync(path.dirname(file), { recursive: true });
    fs.mkdirSync(path.join(OUT, '_svg', path.dirname(name)), { recursive: true });
    fs.writeFileSync(path.join(OUT, '_svg', name + '.svg'), s);
    await page.setViewportSize({ width: w, height: h });
    await page.setContent(`<html><body style="margin:0;background:transparent">${s}</body></html>`);
    await page.locator('svg').screenshot({ path: file, omitBackground: true });
    console.log(name, w + 'x' + h);
  }
  await browser.close();
})();
