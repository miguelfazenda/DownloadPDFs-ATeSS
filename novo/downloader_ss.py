from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import time

class DownloaderSS:
    @staticmethod
    def download_extrato_remuneracoes(driver, ano, mes, expect_download, wait_for_download_finish, gen_novo_nome_ficheiro, declaracao, estrutura_nomes_ficheiros):
        driver.get("https://app.seg-social.pt/ptss/gr/pesquisa/consultarDR?dswid=7064&frawMenu=1")
        ano_mes_str = f"{ano}-{'%02d' % mes}"
        ano_mes_dia_str = f"{ano}-{'%02d' % mes}-01"
        driver.execute_script(f"document.getElementById('dadosPesquisaDeclaracoes:dataReferenciaFimMonthPicker:calendar_input').value = '{ano_mes_str}'")
        driver.execute_script(f"document.getElementById('dadosPesquisaDeclaracoes:dataReferenciaInicioMonthPicker:calendar_input').value = '{ano_mes_str}'")
        driver.execute_script(f"document.getElementById('dadosPesquisaDeclaracoes:dataEntregaInicio:calendar_input').value = '{ano_mes_dia_str}'")
        DownloaderSS.button_run_onclick(driver, (By.ID, "dadosPesquisaDeclaracoes:pesquisa"))
        time.sleep(0.2)
        WebDriverWait(driver, 10).until(EC.invisibility_of_element_located((By.ID, "j_idt28_blocker")))
        if driver.find_elements(By.CLASS_NAME, "ui-datatable-empty-message"):
            return  # No declarations found
        num_extratos = len(driver.find_elements(By.XPATH, "//*[@id='formListaDeclaracoes:tabelaDeclaracoes_data']/*"))
        for i in range(num_extratos):
            expect_download()
            DownloaderSS.button_run_onclick(driver, (By.ID, f"formListaDeclaracoes:tabelaDeclaracoes:{i}:imprimirExtrato"))
            wait_for_download_finish(gen_novo_nome_ficheiro(estrutura_nomes_ficheiros.SS_ExtratoRemun), declaracao.SS_ExtratoRemun, mes)
            time.sleep(1)

    @staticmethod
    def button_run_onclick(driver, by):
        element = driver.find_element(*by)
        on_click_code = element.get_attribute('onclick')
        if on_click_code:
            driver.execute_script(on_click_code)
        else:
            href = element.get_attribute('href')
            driver.get(href)
