Készítsünk programot, amellyel a következő két személyes játékot lehet játszani.
Adott egy 𝑛 × 𝑛 mezőből álló tábla, ahol egy menekülő és egy támadó játékos
helyezkedik el.
Kezdetben a menekülő játékos figurája középen van, míg a támadó figurái a négy
sarokban helyezkednek el. A játékosok felváltva lépnek. A figurák vízszintesen,
illetve függőlegesen mozoghatnak 1-1 mezőt, de egymásra nem léphetnek. A
támadó játékos célja, hogy adott lépésszámon (4𝑛) belül bekerítse a menekülő
figurát, azaz a menekülő ne tudjon lépni.
A program biztosítson lehetőséget új játék kezdésére a táblaméret (3 × 3, 5 × 5,
7 × 7) és így a lépésszám (12, 20, 28) megadásával, folyamatosan jelenítse meg a
lépések számát, és ismerje fel, ha vége a játéknak. Ekkor jelenítse meg, melyik
játékos győzött, majd kezdjen automatikusan új játékot. Ezen felül legyen
lehetőség a játék elmentésére, valamint betöltésére.

Követelmények: A megvalósításnak felhasználóbarátnak, és könnyen kezelhetőnek kell lennie. A szerkezetében törekednie kell az objektumorientált szemlélet megtartására. A programot MVVM architektúrában kell felépíteni. A modell funkcionalitását egységtesztek segítségével kell ellenőrizni. A megvalósítást legalább két választott platformra el kell elkészíteni.
Dokumentáció: Tartalmaznia kell a feladat elemzését, felhasználói eseteit (UML felhasználói esetek diagrammal), a program szerkezetének leírását (UML osztálydiagrammal), valamint a tesztesetek ismertetését.