> Actualización de balance (11/10/2026): las fórmulas y límites actuales están en [Auditoría general y estadísticas](AuditoriaGeneralYEstadisticas.md), sección D. Esa sección reemplaza las cifras anteriores de suerte y defensa de este documento.

# Suerte y ataques multi-hit

**Editor > Suerte y fallos de ataques** abre el archivo CombatAccuracySettings. Cada personaje tiene un porcentaje de fallo normal y especial: 0 nunca falla y 100 siempre falla. Enabled Luck desactiva todo el azar de acierto. Un fallo no resta vida ni dispara el feedback de dano, pero el ataque y el turno siguen normalmente.

Valores iniciales: GamerBoy 10% normal, Tapaku 10%, Hongo 10%, Soldado 12%, Big Tapaku 15%. Los especiales de Soldado y Big Tapaku fallan 25%. Los valores se editan independientemente. La granada fallada pasa a un lado de la camara.

En Hacks puedes agregar reglas con el nombre de clase del HACK, activar Affected By Luck y definir Miss Percent. Unlisted Hacks Use Luck permite aplicar el porcentaje general a HACK sin regla propia. El unico HACK actual es JUMP, que siempre queda fuera del azar, incluso con una regla de 100%: conserva su evaluacion de saltos/ritmo.

**Editor > Enemigos** incluye Big Tapaku Special Hits (3), Special Damage (1 HP por golpe) y Special Hit Interval (0.2 s). Cada golpe tiene su propia tirada y su propio sonido/impacto cuando acierta; el especial recorre sus seis cuadros. El total maximo predeterminado es 3 HP. Si GamerBoy cae, los golpes restantes y los ataques de otros enemigos se cancelan.
