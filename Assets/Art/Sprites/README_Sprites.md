# DeckBound: sprite-uri pentru lupta de bază

Toate sunt PNG cu transparență. `_preview_combat.png` arată cum arată puse împreună (mockup, nu e asset).

| Folder | Fișier | Mărime | Folosire |
|---|---|---|---|
| Characters | `hero_bruiser.png` | 512x512 | Bruiser, privește spre dreapta |
| Characters | `enemy_cultist.png` | 512x512 | Inamicul (cultist cu seceră), privește spre stânga |
| Cards | `card_frame_attack.png` / `card_frame_skill.png` | 400x560 | Rama cărții (roșu = Atac, albastru = Skill). Fereastra artei e goală (transparentă). |
| Cards | `card_back.png` | 400x560 | Spatele cărții (draw/discard pile) |
| Cards | `art_strike.png`, `art_defend.png`, `art_bloodstrike.png` | 480x320 | Arta pentru Lovitură, Apărare, Lovitură Sângeroasă |
| Icons | `icon_energy`, `icon_armor`, `icon_hp`, `icon_intent_attack`, `icon_draw_pile`, `icon_discard_pile` | 128x128 | Iconițe UI |
| UI | `bar_frame.png` (320x40) + `bar_fill.png` (304x24) | | Bară de HP: fill-ul e alb, îl colorezi din Image > Color |
| UI | `button_end_turn.png` | 280x90 | Butonul „Termină tura” (textul îl pui cu TextMeshPro) |
| UI | `intent_bubble.png` | 160x80 | Fundal pentru intenția inamicului |
| Backgrounds | `bg_crypt.png` | 1920x1080 | Fundalul luptei |

Textele (nume, cost, descriere) nu sunt desenate pe rame, ca să le pui din cod cu TextMeshPro.

## Import în Unity
1. Copiază folderul `Sprites` în `Assets/Art/` (sau unde vrei în `Assets`). Folderul `_svg` și `_preview_combat.png` nu trebuie copiate.
2. Selectează toate PNG-urile din Project. În Inspector:
   - **Texture Type**: `Sprite (2D and UI)`
   - **Sprite Mode**: `Single`
   - **Alpha Is Transparency**: bifat
   - **Mesh Type**: `Full Rect` (mai bine pentru UI)
   - **Compression**: `None` sau `High Quality` (altfel apar artefacte pe contururi)
   - Apasă **Apply**.
3. Pentru `bg_crypt.png` setează **Max Size** la 2048 ca să nu fie micșorat.
4. Pentru `bar_frame`, `bar_fill`, `button_end_turn` și `intent_bubble`: deschide **Sprite Editor** și pune border-uri (ex. 10 px stânga/dreapta) ca să se poată întinde fără deformare (Image Type = `Sliced`).

## Cum asamblezi o carte (UI Canvas)
Prefab `Card` (RectTransform 400x560), copii în ordinea asta:
1. `Art` (Image, arta cărții): poziție x 40, y 70 de la colțul stânga-sus, mărime 320x214.
2. `Frame` (Image, rama potrivită tipului) peste ea, pe toată cartea.
3. `Cost` (TextMeshPro) centrat pe globul din stânga-sus (centru 48, 48).
4. `Name` (TextMeshPro) pe banda colorată, y 296–340.
5. `Description` (TextMeshPro) în caseta de jos: x 46, y 350, 308x168.

Pe scenă: eroul în stânga, inamicul în dreapta (ambele cu SpriteRenderer, sau Image în Canvas), bara de HP sub fiecare, iconița de intenție deasupra inamicului.

## Modificări
În `_svg/` sunt sursele SVG și scriptul `generate_sprites.js` care le-a generat. Le poți edita (culori, forme) și redeschide în orice editor SVG (Inkscape, Figma).
