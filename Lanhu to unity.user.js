// ==UserScript==
// @name         Lanhu to unity
// @namespace    http://tampermonkey.net/
// @version      2024-11-19
// @description  Lanhu to unity
// @author       Chenzihan
// @match        https://lanhuapp.com/*
// @icon         https://www.google.com/s2/favicons?sz=64&domain=tampermonkey.net
// @grant        none
// ==/UserScript==

(function() {
    'use strict';

    // 创建 MutationObserver 以监视DOM变化
    const observer = new MutationObserver(mutations => {
        mutations.forEach(mutation => {
            if (mutation.addedNodes.length) {
                mutation.addedNodes.forEach(node => {
                    if (node.nodeType === Node.ELEMENT_NODE) {
                        // 检查新添加的节点是否包含 class="annotation_item"
                        const annotationItems = node.querySelectorAll('.annotation_item>.subtitle');
                        annotationItems.forEach(item => addButton(item));
                    }
                });
            }
        });
    });

    // 添加按钮的函数
    function addButton(item) {
        // 创建新按钮
        const button = document.createElement('button');
        button.textContent = '复制位置信息'; // 按钮文本
        button.style.marginTop = '10px'; // 设置按钮上边距
        button.style.color = 'red'

        // 给按钮添加点击事件，提取位置信息和大小信息
        button.addEventListener('click', function() {
            const positions = [];
            const sizes = [];

            // 查找“位置”项
            var positionTitle = document.querySelector("#detail_container > div.mu-paper.mu-drawer.mu-paper-round.mu-paper-2.open.right.info.info-drawer > div.annotation_container_b > div.annotation_container.lanhu_scrollbar.flag-pl > div.annotation_item > ul > li:nth-child(2) > div.item_title");
            if(!positionTitle) {
                positionTitle = document.querySelector("#detail_container > div.mu-paper.mu-drawer.mu-paper-round.mu-paper-2.open.right.info.info-drawer > div.annotation_container_b > div.annotation_container.lanhu_scrollbar.flag-ps > div:nth-child(1) > ul > li:nth-child(2) > div.item_title")
            }

            const positionItems = positionTitle.closest('li').querySelectorAll('.item_two .two');
            positionItems.forEach(pos => positions.push(pos.textContent.trim().replace(/px$/, '')));

            // 查找“大小”项
            var sizeTitle = document.querySelector("#detail_container > div.mu-paper.mu-drawer.mu-paper-round.mu-paper-2.open.right.info.info-drawer > div.annotation_container_b > div.annotation_container.lanhu_scrollbar.flag-pl > div.annotation_item > ul > li:nth-child(3) > div.item_title");
            if(!sizeTitle) {
                sizeTitle = document.querySelector("#detail_container > div.mu-paper.mu-drawer.mu-paper-round.mu-paper-2.open.right.info.info-drawer > div.annotation_container_b > div.annotation_container.lanhu_scrollbar.flag-ps > div:nth-child(1) > ul > li:nth-child(3) > div.item_title")
            }

            const sizeItems = sizeTitle.closest('li').querySelectorAll('.item_two .two');
            sizeItems.forEach(size => sizes.push(size.textContent.trim().replace(/px$/, '')))

            var colorTitle = document.querySelector("#detail_container > div.mu-paper.mu-drawer.mu-paper-round.mu-paper-2.open.right.info.info-drawer > div.annotation_container_b > div.annotation_container.lanhu_scrollbar.flag-pl > div:nth-child(2) > div.annotation_item > div > div.layer_color > div.layer_color_ul.color_list_wrap > ul > li > div > div.color_item.color_var > div > div.layer-info-var > div > div > span:nth-child(1)")
            if(!colorTitle) {
                colorTitle = document.querySelector("#detail_container > div.mu-paper.mu-drawer.mu-paper-round.mu-paper-2.open.right.info.info-drawer > div.annotation_container_b > div.annotation_container.lanhu_scrollbar.flag-ps > div.annotation_item.\\32  > ul > li:nth-child(4) > div > div.color_item.color_var > div > div.layer-info-var > div > div > span:nth-child(1)")
            }

            // 显示提取的信息
            const message = `${positions.join(',')},${sizes.join(',')},${colorTitle == undefined ? 0 : colorTitle.textContent}`;

            // 创建一个临时 textarea 元素
            const tempTextArea = document.createElement('textarea');
            tempTextArea.value = message; // 设置其值为要复制的文本
            document.body.appendChild(tempTextArea); // 将其添加到文档中

            // 选择文本
            tempTextArea.select();
            tempTextArea.setSelectionRange(0, 99999); // 对于移动设备

            // 执行复制操作
            document.execCommand('copy');

            // 移除临时 textarea
            document.body.removeChild(tempTextArea);
        });

        // 检查项目中是否已有按钮，避免重复添加
        if (!item.querySelector('button')) {
            item.appendChild(button);
        }
    }

    // 初始检查现有的 annotation_item
    document.querySelectorAll('.annotation_item>.subtitle').forEach(item => addButton(item));

    // 开始观察整个文档
    observer.observe(document.body, { childList: true, subtree: true });
})();